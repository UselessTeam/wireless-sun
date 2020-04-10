using System;
using Godot;

public abstract class HoverableItem : _GUIItem {
    public override void _Ready () {
        this.Connect ("mouse_entered", this, "_on_InventoryItem_mouse_entered");
        this.Connect ("mouse_exited", this, "_on_InventoryItem_mouse_exited");
    }
    // Mouse hover
    public void _on_InventoryItem_mouse_entered () {
        GetNode<CanvasItem> ("Holder/Hover").Show ();
    }
    public void _on_InventoryItem_mouse_exited () {
        GetNode<CanvasItem> ("Holder/Hover").Hide ();
    }
}