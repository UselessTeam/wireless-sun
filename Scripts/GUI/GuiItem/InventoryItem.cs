using System;
using Godot;

public class InventoryItem : SelectableItem {
	// Action to perform when double click
	byte index = byte.MaxValue;
	public override void DoubleClick () {
		GameRoot.inventory.Use (index);
	}
	public void Display (InventorySlot slot) {
		index = slot.index;
		Display (slot.slot);
	}
}