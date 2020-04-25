using System;
using Godot;

public class PauseMenu : GUIWindow {
    public override void _Ready () {
        if (Network.IsConnectionStarted && !Gameplay.Instance.IsNetworkMaster ()) {
            GetNode<Button> ("PanelContainer/VBoxContainer/Save").Disabled = true;
            GetNode<Button> ("PanelContainer/VBoxContainer/Quit").Text = "Quit";
        }
    }

    public void _Resume () { Minimise (); }

    void _Save () {
        Save.SaveGame ();
    }
    void _Quit () {
        if (!Network.IsConnectionStarted || Gameplay.Instance.IsNetworkMaster ())
            Save.SaveGame ();
        _Resume ();
        Network.Instance.DisconnectNetwork ();
        GameRoot.LoadMenuScene ();
    }

    public override void Maximise () {
        GetTree ().Paused = true;
        this.Show ();
    }
    public override void Minimise () {
        this.Hide ();
        GetTree ().Paused = false;
    }

    bool canResume = false;
    public override void _Input (InputEvent _input) {
        if (Visible)
            if (_input.IsActionReleased ("pause")) {
                canResume = true;
            }
        if (canResume && _input.IsActionPressed ("pause")) {
            canResume = false;
            _Resume ();
        }
    }
}