using System;
using Godot;

public abstract class HoverableItem : _GUIItem {
    public override void _Ready () {
        this.Connect ("mouse_entered", this, "_on_InventoryItem_mouse_entered");
        this.Connect ("mouse_exited", this, "_on_InventoryItem_mouse_exited");

    }
    // Mouse hover
    public void _on_InventoryItem_mouse_entered () {
        if (MySlot.item != Item.ItemId.NULL) {
            GetNode<CanvasItem> ("Holder/Hover").Show ();
            GetNode<Label> ("Holder/Hover/PanelContainer/Label").Text = MySlot.ToString ();
        }
    }
    public void _on_InventoryItem_mouse_exited () {
        GetNode<CanvasItem> ("Holder/Hover").Hide ();
    }
}