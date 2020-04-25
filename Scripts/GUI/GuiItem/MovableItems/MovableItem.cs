using System;
using Godot;

public abstract class MovableItem : SelectableItem {

    public override void _Ready () {
        base._Ready ();
        this.Connect ("gui_input", this, "_on_MovableItem_gui_input");

        this.Connect ("mouse_entered", this, "_on_mouse_entered");
        this.Connect ("mouse_exited", this, "_on_mouse_exited");
    }

    public static MovableItem movingGuiItem = null;

    Sprite movingSprite = null;

    public void _on_MovableItem_gui_input (InputEvent _input) {
        if (_input is InputEventMouseMotion && Input.IsActionPressed ("ui_select") && !MySlot.item.IsNull ()) {
            if (movingSprite == null) {
                movingSprite = ((PackedScene) ResourceLoader.Load (GetNode ("Holder").Filename)).Instance () as Sprite;
                movingSprite.Modulate = new Color (0, 0, 0, 0.5f);
                GetParent ().AddChild (movingSprite);
                movingSprite.Position = this.RectPosition + GetNode<Sprite> ("Holder").Position;
                movingGuiItem = this;
            }
            movingSprite.Position += (_input as InputEventMouseMotion).Relative;
        }
        if (_input is InputEventMouseMotion) {}
        if (_input.IsActionReleased ("ui_select")) {}
    }

    public void _on_mouse_entered () {
        if (movingGuiItem != null && movingGuiItem != this && !Input.IsActionPressed ("ui_select")) {
            MoveIn (movingGuiItem);
            movingGuiItem = null;
        }
    }
    public void _on_mouse_exited () {
        if (movingSprite != null) {
            movingSprite.QueueFree ();
            movingSprite = null;
        }
    }

    public abstract void MoveIn (SelectableItem item);
}