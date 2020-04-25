using System.Collections.Generic;
using Godot;
using Item;

public class InventoryGrid : GridContainer {
	private PackedScene packedInventoryItem = (PackedScene) ResourceLoader.Load ("res://Nodes/GUI/GuiItem/InventoryItem.tscn");

	public override void _Ready () {}

	private List<InventoryItem> items = new List<InventoryItem> ();
	public void Display (InventorySlot[] stacks) {
		if (items.Count > stacks.Length) {
			for (int i = stacks.Length; i < items.Count; i++) {
				items[i].QueueFree ();
			}
			items.RemoveRange (stacks.Length, items.Count - stacks.Length);
		} else if (items.Count < stacks.Length) {
			for (int i = items.Count; i < stacks.Length; i++) {
				InventoryItem item = (InventoryItem) packedInventoryItem.Instance ();
				items.Add (item);
				AddChild (item);
			}
		}
		for (int i = 0; i < items.Count; i++) {
			items[i].Display (stacks[i]);
		}
	}
}