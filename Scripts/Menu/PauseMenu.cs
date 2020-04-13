using System;
using Godot;

public class PauseMenu : Control {
    public override void _Ready () {
        if (Network.IsConnectionStarted && !Gameplay.Instance.IsNetworkMaster ()) {
            GetNode<Button> ("PanelContainer/VBoxContainer/Save").Disabled = true;
            GetNode<Button> ("PanelContainer/VBoxContainer/Quit").Text = "Quit";
        }
    }
    public void _Resume () {
        this.Hide ();
        GetTree ().Paused = false;
    }
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
}