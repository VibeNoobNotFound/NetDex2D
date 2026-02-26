using Godot;

public partial class PauseMenu : Control
{
    public override void _Ready()
    {
        Hide();
        
        var resumeBtn = GetNode<Button>("PanelContainer/VBoxContainer/ResumeButton");
        resumeBtn.Pressed += OnResumePressed;

        var settingsBtn = GetNode<Button>("PanelContainer/VBoxContainer/SettingsButton");
        settingsBtn.Pressed += OnSettingsPressed;

        var leaveBtn = GetNode<Button>("PanelContainer/VBoxContainer/LeaveButton");
        leaveBtn.Pressed += OnLeavePressed;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel")) // Escape key
        {
            Visible = !Visible;
            // GetTree().Paused = Visible; // If we want to physically pause
        }
    }

    private void OnResumePressed()
    {
        Hide();
        // GetTree().Paused = false;
    }

    private void OnSettingsPressed()
    {
        GD.Print("Settings from pause menu not fully implemented.");
    }

    private void OnLeavePressed()
    {
        Hide();
        // GetTree().Paused = false;
        NetworkManager.Instance.DisconnectSession("Left match");
        GameManager.Instance?.LoadMainMenu();
    }
}
