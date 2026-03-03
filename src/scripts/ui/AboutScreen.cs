using Godot;
using NetDex.Managers;
using NetDex.UI.Polish;
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
    private Control _mainPanel = null!;
    private VBoxContainer _vbox = null!;

    public override void _Ready()
    {
        VisibilityChanged += OnVisibilityChanged;
        InitializeUi();
    }

    private void InitializeUi()
    {
        var panelPath = "ScrollContainer/MarginContainer/CenterContainer/MainPanel";
        _mainPanel = GetNodeOrNull<Control>(panelPath) ?? new PanelContainer();

        var basePath = "ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer";
        _vbox = GetNodeOrNull<VBoxContainer>(basePath) ?? new VBoxContainer();
        if (_vbox == null)
        {
            GD.PushError($"AboutScreen init failed: missing VBoxContainer at '{basePath}'.");
            return;
        }

        _gameNameLabel = _vbox.GetNodeOrNull<Label>("GameNameLabel") ?? new Label();
        _versionLabel = _vbox.GetNodeOrNull<Label>("VersionLabel") ?? new Label();
        _platformLabel = _vbox.GetNodeOrNull<Label>("PlatformLabel") ?? new Label();
        _developerLabel = _vbox.GetNodeOrNull<Label>("DeveloperLabel") ?? new Label();
        _copyrightLabel = _vbox.GetNodeOrNull<Label>("CopyrightLabel") ?? new Label();
        _repoLabel = _vbox.GetNodeOrNull<Label>("RepoUrlLabel") ?? new Label();
        _releasesLabel = _vbox.GetNodeOrNull<Label>("ReleasesUrlLabel") ?? new Label();

        _updateStatusLabel = _vbox.GetNodeOrNull<Label>("UpdateStatusLabel") ?? new Label();
        _latestVersionLabel = _vbox.GetNodeOrNull<Label>("LatestVersionLabel") ?? new Label();

        _updateActionButton = _vbox.GetNodeOrNull<Button>("UpdateActionButton") ?? new Button();
        _skipUpdateButton = _vbox.GetNodeOrNull<Button>("SkipUpdateButton") ?? new Button();
        _backButton = _vbox.GetNodeOrNull<Button>("BackButton") ?? new Button();

        var checkUpdatesButton = _vbox.GetNodeOrNull<Button>("CheckUpdatesButton");
        if (checkUpdatesButton != null)
        {
            checkUpdatesButton.Pressed += OnCheckUpdatesPressed;
        }

        _updateActionButton.Pressed += OnUpdateActionPressed;
        _skipUpdateButton.Pressed += OnSkipUpdatePressed;

        var openRepoButton = _vbox.GetNodeOrNull<Button>("OpenRepoButton");
        if (openRepoButton != null)
        {
            openRepoButton.Pressed += OnOpenRepoPressed;
        }

        var openReleasesButton = _vbox.GetNodeOrNull<Button>("OpenReleasesButton");
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

        var copyright = ProjectSettings.GetSetting("application/config/copyright_notice", "(c) NetDex").AsString();
        if (string.IsNullOrWhiteSpace(copyright))
        {
            copyright = "(c) NetDex";
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
            UiFeedbackService.Instance?.ApplyStatusLabelStyle(_updateStatusLabel, UiSeverity.Warning);
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

        var severity = UiFeedbackService.InferSeverity(updater.StatusMessage);
        UiFeedbackService.Instance?.ApplyStatusLabelStyle(_updateStatusLabel, severity);
    }

    private void OnCheckUpdatesPressed()
    {
        UiFeedbackService.Instance?.SetLoading(true, "Checking for updates...");
        UpdateManager.Instance?.CheckForUpdates(manual: true);
    }

    private void OnUpdateActionPressed()
    {
        UiFeedbackService.Instance?.SetLoading(true, "Preparing update...");
        UpdateManager.Instance?.PerformUpdateAction();
    }

    private void OnSkipUpdatePressed()
    {
        UpdateManager.Instance?.SkipCurrentUpdate();
        UiFeedbackService.Instance?.ShowToast("Update skipped", UiSeverity.Info, 1.8);
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

        _mainPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _mainPanel.Scale = new Vector2(0.97f, 0.97f);

        var panelTween = CreateTween();
        panelTween.SetParallel(true);
        panelTween.TweenProperty(_mainPanel, "modulate:a", 1f, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        panelTween.TweenProperty(_mainPanel, "scale", Vector2.One, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        var idx = 0;
        foreach (Node child in _vbox.GetChildren())
        {
            if (child is not Control control)
            {
                continue;
            }

            control.Modulate = new Color(1f, 1f, 1f, 0f);
            var tween = CreateTween();
            tween.TweenProperty(control, "modulate:a", 1f, 0.16f)
                .SetDelay(0.04f * idx)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            idx++;
        }
    }

    private void OnUpdateStatusChanged(string state, string message)
    {
        RefreshUpdateUi();

        var lower = state.ToLowerInvariant();
        var loading = lower is "checking" or "downloading" or "installing";
        UiFeedbackService.Instance?.SetLoading(loading, message);

        if (!loading && UiFeedbackService.InferSeverity(message) == UiSeverity.Error)
        {
            UiFeedbackService.Instance?.ShowToast(message, UiSeverity.Error, 2.4);
        }
    }

    private void OnUpdateAvailable(string version, string platformActionLabel)
    {
        _updateActionButton.Text = platformActionLabel;
        RefreshUpdateUi();
        UiFeedbackService.Instance?.ShowBanner($"Update available: {version}", UiSeverity.Success, 2.2);
    }

    private void OnUpdateIssueRaised(int issueCode, string message)
    {
        RefreshUpdateUi();
        UiFeedbackService.Instance?.SetLoading(false);
        UiFeedbackService.Instance?.ShowToast(message, UiSeverity.Error, 2.4);
    }
}
