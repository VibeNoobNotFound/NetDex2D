using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using NetDex.Updates.Installers;

namespace NetDex.Updates;

public partial class UpdateManager : Node
{
    private const string UpdaterConfigPath = "user://updater.cfg";
    private const string DefaultRepositoryUrl = "https://github.com/NoobNotFound/net-dex";
    private const string DefaultReleasesUrl = "https://github.com/NoobNotFound/net-dex/releases";
    private const string DefaultRepoOwner = "NoobNotFound";
    private const string DefaultRepoName = "net-dex";

    private const string MacAssetName = "netdex-macos-universal.zip";
    private const string AndroidAssetName = "netdex-android-arm64.apk";

    private const double AutoCheckIntervalSeconds = 6 * 60 * 60;

    public static UpdateManager Instance { get; private set; } = null!;

    public UpdateState CurrentState { get; private set; } = UpdateState.Idle;
    public string StatusMessage { get; private set; } = "Updates not checked yet.";
    public string CurrentVersion { get; private set; } = "0.0.0";
    public UpdateReleaseInfo? LatestRelease { get; private set; }
    public bool IsUpdateAvailable { get; private set; }

    public string RepositoryUrl { get; private set; } = DefaultRepositoryUrl;
    public string ReleasesUrl { get; private set; } = DefaultReleasesUrl;

    public UpdatePlatform RuntimePlatform { get; private set; } = UpdatePlatform.Unsupported;

    private readonly GitHubReleaseProvider _provider = new();
    private readonly Dictionary<UpdatePlatform, IUpdateInstaller> _installers = new();

    private string _repoOwner = DefaultRepoOwner;
    private string _repoName = DefaultRepoName;

    private bool _isChecking;
    private bool _isInstalling;

    private double _lastCheckedUnix;
    private string _skippedVersion = string.Empty;
    private string _lastKnownLatest = string.Empty;
    private string _downloadPathTemp = string.Empty;

    [Signal]
    public delegate void UpdateStatusChangedEventHandler(string state, string message);

    [Signal]
    public delegate void UpdateAvailableEventHandler(string version, string platformActionLabel);

    [Signal]
    public delegate void UpdateIssueRaisedEventHandler(int issueCode, string message);

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;

        RuntimePlatform = DetectRuntimePlatform();
        CurrentVersion = VersionComparer.NormalizeTagToVersion(ProjectSettings.GetSetting("application/config/version", "0.0.0").AsString());

        RepositoryUrl = GetProjectSettingString("application/config/repository_url", DefaultRepositoryUrl);
        ReleasesUrl = GetProjectSettingString("application/config/releases_url", DefaultReleasesUrl);

        if (!TryParseOwnerRepoFromUrl(RepositoryUrl, out _repoOwner, out _repoName))
        {
            _repoOwner = DefaultRepoOwner;
            _repoName = DefaultRepoName;
        }

        _installers[UpdatePlatform.MacOS] = new MacOsUpdateInstaller();
        _installers[UpdatePlatform.Android] = new AndroidBrowserInstaller();
        _installers[UpdatePlatform.IOS] = new IosManualInstaller();

        LoadUpdaterConfig();
        _ = AutoCheckIfNeededAsync();
    }

    public string GetActionLabel()
    {
        return _installers.TryGetValue(RuntimePlatform, out var installer)
            ? installer.ActionLabel
            : "Open Update";
    }

    public string GetLatestVersionLabel()
    {
        return LatestRelease?.TagName ?? _lastKnownLatest;
    }

    public void CheckForUpdates(bool manual = false)
    {
        _ = CheckForUpdatesAsync(manual);
    }

    public async Task CheckForUpdatesAsync(bool manual = false)
    {
        if (_isChecking || _isInstalling)
        {
            return;
        }

        _isChecking = true;
        SetState(UpdateState.Checking, "Checking for updates...");

        try
        {
            var query = await _provider.FetchLatestReleaseAsync(this, _repoOwner, _repoName);
            _lastCheckedUnix = Time.GetUnixTimeFromSystem();

            if (!query.Success || query.Release == null)
            {
                var code = query.IssueCode ?? UpdateIssueCode.NetworkError;
                SetError(code, query.Message);
                SaveUpdaterConfig();
                return;
            }

            LatestRelease = query.Release;
            _lastKnownLatest = query.Release.TagName;
            SaveUpdaterConfig();

            if (!VersionComparer.IsNewer(query.Release.Version, CurrentVersion))
            {
                IsUpdateAvailable = false;
                SetState(UpdateState.UpToDate, $"You are up to date ({CurrentVersion}).");
                return;
            }

            var platformAsset = ResolvePlatformAsset(query.Release, RuntimePlatform);
            if ((RuntimePlatform == UpdatePlatform.MacOS || RuntimePlatform == UpdatePlatform.Android) && platformAsset == null)
            {
                IsUpdateAvailable = false;
                SetError(UpdateIssueCode.AssetMissing, "Latest release does not contain required asset for this platform.");
                return;
            }

            if (RuntimePlatform == UpdatePlatform.MacOS && !ChecksumVerifier.TryParseSha256Digest(platformAsset!.Digest, out _))
            {
                IsUpdateAvailable = false;
                SetError(UpdateIssueCode.DigestMissing, "Release digest is missing or invalid for macOS update asset.");
                return;
            }

            IsUpdateAvailable = true;
            var skippedSuffix = string.Equals(_skippedVersion, query.Release.TagName, StringComparison.Ordinal)
                ? " (previously skipped)"
                : string.Empty;
            SetState(UpdateState.UpdateAvailable, $"Update {query.Release.TagName} is available{skippedSuffix}.");
            EmitSignal(SignalName.UpdateAvailable, query.Release.TagName, GetActionLabel());

            if (manual)
            {
                _skippedVersion = string.Empty;
                SaveUpdaterConfig();
            }
        }
        finally
        {
            _isChecking = false;
        }
    }

    public void PerformUpdateAction()
    {
        _ = PerformUpdateActionAsync();
    }

    public async Task PerformUpdateActionAsync()
    {
        if (_isChecking || _isInstalling)
        {
            return;
        }

        if (!IsUpdateAvailable || LatestRelease == null)
        {
            SetState(UpdateState.Idle, "No update available right now.");
            return;
        }

        if (!_installers.TryGetValue(RuntimePlatform, out var installer))
        {
            SetError(UpdateIssueCode.UnsupportedPlatform, "Updates are not supported on this platform.");
            return;
        }

        _isInstalling = true;

        try
        {
            if (RuntimePlatform == UpdatePlatform.MacOS)
            {
                SetState(UpdateState.Downloading, "Downloading update package...");
                _downloadPathTemp = "user://updates/netdex-macos-universal.zip.download";
                SaveUpdaterConfig();
            }
            else
            {
                SetState(UpdateState.Installing, "Opening update destination...");
            }

            var platformAsset = ResolvePlatformAsset(LatestRelease, RuntimePlatform);
            var result = await installer.ExecuteAsync(this, LatestRelease, platformAsset);
            if (!result.Success)
            {
                SetError(result.IssueCode ?? UpdateIssueCode.InstallFailed, result.Message);
                return;
            }

            _skippedVersion = string.Empty;
            _downloadPathTemp = string.Empty;
            SaveUpdaterConfig();

            if (RuntimePlatform == UpdatePlatform.MacOS)
            {
                SetState(UpdateState.Installing, result.Message);
                GetTree().Quit();
                return;
            }

            SetState(UpdateState.Idle, result.Message);
        }
        finally
        {
            _isInstalling = false;
        }
    }

    public void SkipCurrentUpdate()
    {
        if (LatestRelease == null)
        {
            return;
        }

        _skippedVersion = LatestRelease.TagName;
        SaveUpdaterConfig();
        SetState(UpdateState.Idle, $"Skipped update {_skippedVersion}. You can install it later.");
    }

    private async Task AutoCheckIfNeededAsync()
    {
        var now = Time.GetUnixTimeFromSystem();
        if (now - _lastCheckedUnix < AutoCheckIntervalSeconds)
        {
            return;
        }

        await CheckForUpdatesAsync(false);
    }

    private void SetState(UpdateState state, string message)
    {
        CurrentState = state;
        StatusMessage = message;
        EmitSignal(SignalName.UpdateStatusChanged, state.ToString(), message);
    }

    private void SetError(UpdateIssueCode code, string message)
    {
        CurrentState = UpdateState.Error;
        StatusMessage = message;
        EmitSignal(SignalName.UpdateStatusChanged, CurrentState.ToString(), message);
        EmitSignal(SignalName.UpdateIssueRaised, (int)code, message);
    }

    private static UpdateAssetInfo? ResolvePlatformAsset(UpdateReleaseInfo release, UpdatePlatform platform)
    {
        if (platform == UpdatePlatform.MacOS)
        {
            return FindAssetByName(release, MacAssetName);
        }

        if (platform == UpdatePlatform.Android)
        {
            return FindAssetByName(release, AndroidAssetName);
        }

        return null;
    }

    private static UpdateAssetInfo? FindAssetByName(UpdateReleaseInfo release, string expectedName)
    {
        foreach (var asset in release.Assets)
        {
            if (string.Equals(asset.Name, expectedName, StringComparison.OrdinalIgnoreCase))
            {
                return asset;
            }
        }

        return null;
    }

    private void LoadUpdaterConfig()
    {
        var config = new ConfigFile();
        if (config.Load(UpdaterConfigPath) != Error.Ok)
        {
            return;
        }

        _lastCheckedUnix = config.GetValue("updater", "last_checked_unix", 0.0).AsDouble();
        _skippedVersion = config.GetValue("updater", "skipped_version", string.Empty).AsString();
        _lastKnownLatest = config.GetValue("updater", "last_known_latest", string.Empty).AsString();
        _downloadPathTemp = config.GetValue("updater", "download_path_temp", string.Empty).AsString();
    }

    private void SaveUpdaterConfig()
    {
        var config = new ConfigFile();
        config.SetValue("updater", "last_checked_unix", _lastCheckedUnix);
        config.SetValue("updater", "skipped_version", _skippedVersion);
        config.SetValue("updater", "last_known_latest", _lastKnownLatest);
        config.SetValue("updater", "download_path_temp", _downloadPathTemp);
        config.Save(UpdaterConfigPath);
    }

    private static UpdatePlatform DetectRuntimePlatform()
    {
        if (OS.HasFeature("macos"))
        {
            return UpdatePlatform.MacOS;
        }

        if (OS.HasFeature("android"))
        {
            return UpdatePlatform.Android;
        }

        if (OS.HasFeature("ios"))
        {
            return UpdatePlatform.IOS;
        }

        return UpdatePlatform.Unsupported;
    }

    private static bool TryParseOwnerRepoFromUrl(string repositoryUrl, out string owner, out string repo)
    {
        owner = string.Empty;
        repo = string.Empty;

        if (string.IsNullOrWhiteSpace(repositoryUrl) || !Uri.TryCreate(repositoryUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var parts = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            return false;
        }

        owner = parts[0];
        repo = parts[1].Replace(".git", string.Empty, StringComparison.OrdinalIgnoreCase);
        return !string.IsNullOrWhiteSpace(owner) && !string.IsNullOrWhiteSpace(repo);
    }

    private static string GetProjectSettingString(string key, string fallback)
    {
        if (!ProjectSettings.HasSetting(key))
        {
            return fallback;
        }

        var value = ProjectSettings.GetSetting(key, fallback).AsString();
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }
}
