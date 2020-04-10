using System;
using Godot;

public class HoverLabel : Label {
    HoverableItem MyControl { get { return GetParent ().GetParent ().GetParent ().GetParent<HoverableItem> (); } }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {
        MyControl.Connect ("mouse_entered", this, "_on_InventoryItem_mouse_entered");
        Text = MyControl.MyStack.item.ToString ();
    }

    // Mouse hover
    public void _on_InventoryItem_mouse_entered () {
        Text = MyControl.MyStack.item.ToString ();
    }
}