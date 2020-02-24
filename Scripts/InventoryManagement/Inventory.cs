using System.Collections.Generic;
using Godot;
using Item;

public class Inventory : Node2D {
	public List<ItemStack> inventory = new List<ItemStack>();

	[Signal]
	public delegate void inventory_change();
	public override void _Ready () {
		Global.inventory = this;
		GD.Print (Item.Manager.GetItem ("Berry"));
		GD.Print (Item.Manager.GetItem ("ItemQuiExistePas"));
	}

	public void Add(ItemId item, short quantity = 1) {
		for(int i = 0; i < inventory.Count ; i++) {
			if(inventory[i].item == item) {
				inventory[i] = new ItemStack(item, (ushort)(inventory[i].size + quantity));
				EmitSignal(nameof(inventory_change));
				return;
			}
		}
		inventory.Add(new ItemStack(item, (ushort)quantity));
		EmitSignal(nameof(inventory_change));
	}

	public void Remove(ItemId item, short quantity = 1) {
		for(int i = 0; i < inventory.Count ; i++) {
			if(inventory[i].item == item) {
				int newQuantity = inventory[i].size - quantity;
				if(newQuantity > 0) {
					inventory[i] = new ItemStack(item, (ushort)newQuantity);
					EmitSignal(nameof(inventory_change));
					return;
				}
				break;
			}
		}
		GD.PrintErr("[Inventory] Couldn't remove ", quantity, " of ", item.data);
	}
}
