using System;
using Godot;

public class HoverLabel : Label {
    _GUIItem MyControl { get { return GetParent ().GetParent ().GetParent ().GetParent<_GUIItem> (); } }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {
        MyControl.Connect ("mouse_entered", this, "_on_InventoryItem_mouse_entered");
    }

    // Mouse hover
    public void _on_InventoryItem_mouse_entered () {
        Text = MyControl.MySlot.ToString ();
    }
}