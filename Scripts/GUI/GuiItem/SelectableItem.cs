using System;
using Godot;

public abstract class SelectableItem : HoverableItem {

	public override void _Ready () {
		base._Ready ();
		this.Connect ("gui_input", this, "_on_InventoryItem_gui_input");
	}
	bool isSelected = false;

	public void Unselect () {
		isSelected = false;
		GetNode<Sprite> ("Holder/Select").Hide ();
	}
	// Action to perform when selected once
	public void Select () {
		if (MySlot.item != Item.ItemId.NULL) {
			isSelected = true;
			GetNode<Sprite> ("Holder/Select").Show ();
		}
	}
	// Action to perform when double click
	public abstract void DoubleClick ();

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