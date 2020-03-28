using System;
using Godot;

public class CraftingGUI : GUIWindow {
    public override void Maximise () {
        this.Show ();
    }
    public override void Minimise () {
        this.Hide ();
    }

}