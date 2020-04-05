using System;
using Godot;

public abstract class SelectableItem : BlandItem {

	public override void _Ready () {
		this.Connect ("mouse_entered", this, "_on_InventoryItem_mouse_entered");
		this.Connect ("mouse_exited", this, "_on_InventoryItem_mouse_exited");
		this.Connect ("gui_input", this, "_on_InventoryItem_gui_input");
	}
	bool isSelected = false;

	public void Unselect () {
		isSelected = false;
		GetNode<Sprite> ("Holder/Select").Hide ();
	}
	// Action to perform when selected once
	public void Select () {
		isSelected = true;
		GetNode<Sprite> ("Holder/Select").Show ();
	}
	// Action to perform when double click
	public abstract void DoubleClick ();

	// Mouse hover
	public void _on_InventoryItem_mouse_entered () {
		GetNode<CanvasItem> ("Holder/Hover").Show ();
	}
	public void _on_InventoryItem_mouse_exited () {
		GetNode<CanvasItem> ("Holder/Hover").Hide ();
	}

	public void _on_InventoryItem_gui_input (InputEvent _input) {
		if (_input.IsActionPressed ("ui_select")) {
			if (!isSelected) Select ();
		}
		if (_input.GetType () == new InputEventMouseButton ().GetType () &&
			((InputEventMouseButton) _input).Doubleclick)
			DoubleClick ();

	}
	public override void _Input (InputEvent _event) {
		if (isSelected && _event.IsActionPressed ("ui_select")) Unselect ();
	}
}