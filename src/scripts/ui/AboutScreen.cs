using Godot;
using NetDex.Managers;
using NetDex.Updates;

namespace NetDex.UI.Main;

public partial class AboutScreen : Control
{
    private Label _gameNameLabel = null!;
    private Label _versionLabel = null!;
    private Label _platformLabel = null!;
    private Label _developerLabel = null!;
    private Label _copyrightLabel = null!;
    private Label _repoLabel = null!;
    private Label _releasesLabel = null!;

    private Label _updateStatusLabel = null!;
    private Label _latestVersionLabel = null!;
    private Button _updateActionButton = null!;
    private Button _skipUpdateButton = null!;
    private Button _backButton = null!;

    public override void _Ready()
    {
        VisibilityChanged += OnVisibilityChanged;
        InitializeUi();
    }

    private void InitializeUi()
    {
        var basePath = "ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer";
        var vbox = GetNodeOrNull<VBoxContainer>(basePath);
        if (vbox == null)
        {
            GD.PushError($"AboutScreen init failed: missing VBoxContainer at '{basePath}'.");
            return;
        }

        _gameNameLabel = vbox.GetNodeOrNull<Label>("GameNameLabel") ?? new Label();
        _versionLabel = vbox.GetNodeOrNull<Label>("VersionLabel") ?? new Label();
        _platformLabel = vbox.GetNodeOrNull<Label>("PlatformLabel") ?? new Label();
        _developerLabel = vbox.GetNodeOrNull<Label>("DeveloperLabel") ?? new Label();
        _copyrightLabel = vbox.GetNodeOrNull<Label>("CopyrightLabel") ?? new Label();
        _repoLabel = vbox.GetNodeOrNull<Label>("RepoUrlLabel") ?? new Label();
        _releasesLabel = vbox.GetNodeOrNull<Label>("ReleasesUrlLabel") ?? new Label();

        _updateStatusLabel = vbox.GetNodeOrNull<Label>("UpdateStatusLabel") ?? new Label();
        _latestVersionLabel = vbox.GetNodeOrNull<Label>("LatestVersionLabel") ?? new Label();

        _updateActionButton = vbox.GetNodeOrNull<Button>("UpdateActionButton") ?? new Button();
        _skipUpdateButton = vbox.GetNodeOrNull<Button>("SkipUpdateButton") ?? new Button();
        _backButton = vbox.GetNodeOrNull<Button>("BackButton") ?? new Button();

        var checkUpdatesButton = vbox.GetNodeOrNull<Button>("CheckUpdatesButton");
        if (checkUpdatesButton != null)
        {
            checkUpdatesButton.Pressed += OnCheckUpdatesPressed;
        }

        _updateActionButton.Pressed += OnUpdateActionPressed;
        _skipUpdateButton.Pressed += OnSkipUpdatePressed;

        var openRepoButton = vbox.GetNodeOrNull<Button>("OpenRepoButton");
        if (openRepoButton != null)
        {
            openRepoButton.Pressed += OnOpenRepoPressed;
        }

        var openReleasesButton = vbox.GetNodeOrNull<Button>("OpenReleasesButton");
        if (openReleasesButton != null)
        {
            openReleasesButton.Pressed += OnOpenReleasesPressed;
        }

        _backButton.Pressed += OnBackPressed;

        PopulateInfo();
        RefreshUpdateUi();

        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.UpdateStatusChanged += OnUpdateStatusChanged;
            UpdateManager.Instance.UpdateAvailable += OnUpdateAvailable;
            UpdateManager.Instance.UpdateIssueRaised += OnUpdateIssueRaised;
        }
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;

        if (UpdateManager.Instance == null)
        {
            return;
        }

        UpdateManager.Instance.UpdateStatusChanged -= OnUpdateStatusChanged;
        UpdateManager.Instance.UpdateAvailable -= OnUpdateAvailable;
        UpdateManager.Instance.UpdateIssueRaised -= OnUpdateIssueRaised;
    }

    private void PopulateInfo()
    {
        var gameName = ProjectSettings.GetSetting("application/config/name", "NetDex").AsString();
        var version = ProjectSettings.GetSetting("application/config/version", "0.0.0").AsString();

        var developer = ProjectSettings.GetSetting("application/config/developer_name", "NoobNotFound").AsString();
        if (string.IsNullOrWhiteSpace(developer))
        {
            developer = "NoobNotFound";
        }

        var copyright = ProjectSettings.GetSetting("application/config/copyright_notice", "(c) VibeNoobNotFound").AsString();
        if (string.IsNullOrWhiteSpace(copyright))
        {
            copyright = "(c) VibeNoobNotFound";
        }

        var updater = UpdateManager.Instance;
        var platformText = updater != null ? updater.RuntimePlatform.ToString() : "Unknown";
        var repositoryUrl = updater?.RepositoryUrl ?? "-";
        var releasesUrl = updater?.ReleasesUrl ?? "-";

        _gameNameLabel.Text = $"Game: {gameName}";
        _versionLabel.Text = $"Version: {version}";
        _platformLabel.Text = $"Platform: {platformText}";
        _developerLabel.Text = $"Developer: {developer}";
        _copyrightLabel.Text = $"Copyright: {copyright}";
        _repoLabel.Text = $"Repo: {repositoryUrl}";
        _releasesLabel.Text = $"Releases: {releasesUrl}";
    }

    private void RefreshUpdateUi()
    {
        var updater = UpdateManager.Instance;
        if (updater == null)
        {
            _updateStatusLabel.Text = "Updater unavailable.";
            _latestVersionLabel.Text = "Latest version: -";
            _updateActionButton.Disabled = true;
            _skipUpdateButton.Disabled = true;
            return;
        }

        _updateStatusLabel.Text = $"Update Status: {updater.StatusMessage}";
        var latestTag = updater.GetLatestVersionLabel();
        _latestVersionLabel.Text = string.IsNullOrWhiteSpace(latestTag)
            ? "Latest version: unknown"
            : $"Latest version: {latestTag}";

        _updateActionButton.Text = updater.GetActionLabel();
        _updateActionButton.Disabled = !updater.IsUpdateAvailable;
        _skipUpdateButton.Disabled = !updater.IsUpdateAvailable;
    }

    private static void OnCheckUpdatesPressed()
    {
        UpdateManager.Instance?.CheckForUpdates(manual: true);
    }

    private static void OnUpdateActionPressed()
    {
        UpdateManager.Instance?.PerformUpdateAction();
    }

    private static void OnSkipUpdatePressed()
    {
        UpdateManager.Instance?.SkipCurrentUpdate();
    }

    private static void OnOpenRepoPressed()
    {
        var updater = UpdateManager.Instance;
        if (updater == null)
        {
            return;
        }

        OS.ShellOpen(updater.RepositoryUrl);
    }

    private static void OnOpenReleasesPressed()
    {
        var updater = UpdateManager.Instance;
        if (updater == null)
        {
            return;
        }

        OS.ShellOpen(updater.ReleasesUrl);
    }

    private void OnBackPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnFromAbout();
            return;
        }

        GetTree().Root.GetNodeOrNull<GameManager>("/root/GameManager")?.ReturnFromAbout();
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        PopulateInfo();
        RefreshUpdateUi();
    }

    private void OnUpdateStatusChanged(string state, string message)
    {
        RefreshUpdateUi();
    }

    private void OnUpdateAvailable(string version, string platformActionLabel)
    {
        _updateActionButton.Text = platformActionLabel;
        RefreshUpdateUi();
    }

    private void OnUpdateIssueRaised(int issueCode, string message)
    {
        RefreshUpdateUi();
    }
}
