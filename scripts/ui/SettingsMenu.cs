using Godot;

public partial class SettingsMenu : Control
{
    public override void _Ready()
    {
        var backBtn = GetNode<Button>("VBoxContainer/BackButton");
        backBtn.Pressed += OnBackPressed;
    }

    private void OnBackPressed()
    {
        // In a real game we'd differentiate if returning to Main Menu or Pause Menu
        // But for simplicity, let's just go back to MainMenu
        GameManager.Instance?.LoadMainMenu();
    }
}
