using Godot;
using System.Collections.Generic;
using Item;

public class InventoryGrid : GridContainer
{
	private PackedScene packedInventoryItem = (PackedScene)ResourceLoader.Load("res://Nodes/GUI/InventoryItem.tscn");

	private List<InventoryItem> items = new List<InventoryItem>();
	public void Display(List<ItemStack> stacks) {
		GD.Print("Displaying: ", stacks.ToPrint());
		if (items.Count > stacks.Count) {
			for(int i = stacks.Count; i < items.Count; i++) {
				items[i].QueueFree();
			}
			items.RemoveRange(stacks.Count, items.Count - stacks.Count);
		} else if (items.Count < stacks.Count) {
			for(int i = items.Count; i < stacks.Count; i++) {
				InventoryItem item = (InventoryItem)packedInventoryItem.Instance();
				items.Add(item);
				AddChild(item);
			}
		}
		for(int i = 0; i < items.Count; i++) {
			items[i].Display(stacks[i]);
		}
	}
}
