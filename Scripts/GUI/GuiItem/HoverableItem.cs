using System;
using Godot;

public abstract class HoverableItem : _GUIItem {
    Vector2 basePositon;
    Control hoverLabel;

    readonly static float xSafeWidth = 20; // Safe distance around to keep inside the screen 

    public override void _Ready () {
        this.Connect ("mouse_entered", this, "_on_InventoryItem_mouse_entered");
        this.Connect ("mouse_exited", this, "_on_InventoryItem_mouse_exited");
        hoverLabel = GetNode<Control> ("Holder/Hover/LabelContainer");
        basePositon = hoverLabel.RectPosition;
    }

    // Mouse hover
    public void _on_InventoryItem_mouse_entered () {
        if (MySlot.item != Item.ItemId.NULL) {
            hoverLabel.GetNode<Label> ("Label").Text = MySlot.ToString (); // Set the text first so the box takes the right size
            GetNode<CanvasItem> ("Holder/Hover").Show ();

            // If the hover text box doesn't fit in the screen, we mode it appropriately
            var screenRect = GameRoot._GUI.GetNode<Control> ("ShowOnGameplay").GetGlobalRect ();
            var newPos = hoverLabel.RectPosition;
            if (screenRect.End.y < hoverLabel.GetGlobalRect ().End.y) {
                newPos.y -= RectSize.y + hoverLabel.RectSize.y; // Shift up
            }
            if (screenRect.End.x < hoverLabel.GetGlobalRect ().End.x + xSafeWidth) {
                newPos.x -= hoverLabel.GetGlobalRect ().End.x - screenRect.End.x + xSafeWidth; //We slightly shift it left
            }
            if (screenRect.Position.x > hoverLabel.GetGlobalRect ().Position.x - xSafeWidth)
                newPos.x += screenRect.Position.x - hoverLabel.GetGlobalRect ().Position.x + xSafeWidth; //We slightly shift it right
            hoverLabel.RectPosition = newPos;

        }
    }
    public void _on_InventoryItem_mouse_exited () {
        GetNode<CanvasItem> ("Holder/Hover").Hide ();
        hoverLabel.RectPosition = basePositon;
        hoverLabel.RectSize -= new Vector2 (0, hoverLabel.RectSize.y);
    }
}