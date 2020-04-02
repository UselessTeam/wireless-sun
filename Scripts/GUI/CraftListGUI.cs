using System;
using Craft;
using Godot;

public class CraftListGUI : GUIWindow {
    private PackedScene packedCraftGUI = (PackedScene) ResourceLoader.Load ("res://Nodes/GUI/Craft.tscn");

    public override void Maximise () {
        this.Show ();
    }
    public override void Minimise () {
        this.Hide ();
    }

    public void Display (string craftLocation) {
        for (int i = 0; i < GetChildCount (); i++) {
            GetChild (i).QueueFree ();
        }
        var crafts = Craft.Manager.GetCraftLocation (craftLocation).crafts;
        foreach (var craft in crafts) {
            var craftGUI = (CraftGUI) packedCraftGUI.Instance ();
            AddChild (craftGUI);
            craftGUI.Display (craft);
        }
    }

}