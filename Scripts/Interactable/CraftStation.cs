using System;
using Godot;

public class CraftStation : InteractionComponent {
    [Export] public string craftLocation = "";
    public CraftListGUI craftingGUI { get { return GetNode<GUI> ("/root/GUI").crafting; } }

    private bool isGuiDisplayed = false;

    public override void _Ready () {
        Connect ("interaction", this, "_on_interaction");
        Connect ("leave_interaction", this, "_on_leave_interaction");
    }

    public void _on_interaction () {
        if (!isGuiDisplayed) {
            craftingGUI.Display (craftLocation);
            craftingGUI.Maximise ();
            isGuiDisplayed = true;
        }
    }

    public void _on_leave_interaction () {
        craftingGUI.Minimise ();
        isGuiDisplayed = false;
    }
}
