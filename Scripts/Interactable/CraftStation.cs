using System;
using Godot;

public class CraftStation : _Interactable {
    [Export] public string craftLocation = "";
    public CraftListGUI craftingGUI { get { return GetNode<GUI> ("/root/GUI").crafting; } }

    private bool isGuiDisplayed = false;

    public override void Interact () {
        if (!isGuiDisplayed) {
            craftingGUI.Display (craftLocation);
            craftingGUI.Maximise ();
            isGuiDisplayed = true;
        }
    }

    public new void _on_Area2D_body_exited (PhysicsBody2D _body) {
        base._on_Area2D_body_exited (_body);
        craftingGUI.Minimise ();
        isGuiDisplayed = false;
    }

}