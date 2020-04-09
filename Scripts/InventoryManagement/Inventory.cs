using System.Collections.Generic;
using Craft;
using Godot;
using Item;

public class Inventory : Node2D {
	public List<ItemStack> inventory = new List<ItemStack> ();

	[Signal]
	public delegate void inventory_change ();
	public override void _Ready () {
		GameRoot.inventory = this;
	}

	public void Add (ItemId item, ushort quantity = 1) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == item) {
				inventory[i] = new ItemStack (item, (ushort) (inventory[i].size + quantity));
				EmitSignal (nameof (inventory_change));
				return;
			}
		}
		inventory.Add (new ItemStack (item, (ushort) quantity));
		EmitSignal (nameof (inventory_change));
	}

	public void Remove (ItemId item, ushort quantity = 1) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == item) {
				int newQuantity = inventory[i].size - quantity;
				if (newQuantity >= 0) {
					inventory[i] = new ItemStack (item, (ushort) newQuantity);
					EmitSignal (nameof (inventory_change));
					return;
				}
				break;
			}
		}
		GD.PrintErr ("[Inventory] Couldn't remove ", quantity, " of ", item.data);
	}
	public bool Contains (ItemStack stack) { return Contains (stack.item, stack.size); }

	public bool Contains (ItemId item, ushort quantity = 1) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == item) {
				return inventory[i].size >= quantity;
			}
		}
		return false;
	}

	public void MakeCraft (CraftId craftId) {
		CraftData craft = Craft.Manager.GetCraft (craftId);
		foreach (Ingredient ingredient in craft.ingredients) {
			Remove (Item.Manager.GetId (ingredient.item));
		};
		Add (Item.Manager.GetId (craft.result), craft.amount);
	}

	public bool CanCraft (CraftId craft) {
		foreach (Ingredient ingredient in Craft.Manager.GetCraft (craft).ingredients) {
			if (!Contains (Item.Manager.GetId (ingredient.item)))
				return false;
		};
		return true;
	}

	public void Use (ItemId item) {
		if (!Contains (new ItemStack (item, 1))) {
			GD.Print ("You don't have this item :", item);
			return;
		}
		if (item.category == Item.Manager.GetCategory ("equipement").id) {
			Remove (item);
			GetNode<_GUIItem> ("/root/GUI/EquipedItem").Display (new ItemStack (item, 0));
		}
		if (item.category == Item.Manager.GetCategory ("food").id) {
			Remove (item);
			GD.Print ("Miam, it's delicious!");
		}
	}
}