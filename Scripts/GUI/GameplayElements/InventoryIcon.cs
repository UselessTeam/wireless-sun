using System;
using Godot;

public class InventoryIcon : Button {
    [Export] string iconLabel = "NoName";
    [Export] GUI.Window menuPointer;

    public override void _Ready () {
        Connect ("pressed", this, nameof (OnPressed));
        GetNode<Label> ("PanelContainer/Label").Text = iconLabel;
    }
    void OnPressed () { GameRoot._GUI.Toggle (menuPointer); }
}