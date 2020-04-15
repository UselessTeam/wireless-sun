using System.Collections.Generic;
using Craft;
using Godot;
using Item;

public class Inventory : Node2D {
	const ushort INVENTORY_SIZE = 24;
	public List<ItemSlot> inventory = new List<ItemSlot> ();

	[Signal]
	public delegate void inventory_change ();
	public override void _Ready () {
		AddToGroup ("SaveNodes");
		GameRoot.inventory = this;
		for (int i = 0; i < INVENTORY_SIZE; i++)
			inventory.Add (Item.Builder.MakeSlot (ItemId.NULL));
		EmitSignal (nameof (inventory_change));
	}

	public void Add (ItemId item, ushort quantity = 1) {
		ItemData data = Item.Manager.GetItem (item);
		if (data.stackSize > 1)
			for (int i = 0; i < inventory.Count; i++) {
				if (inventory[i].item == item) {
					ushort newSize = (ushort) Mathf.Min ((inventory[i] as ItemStack).size + quantity, data.stackSize);
					quantity -= (ushort) (newSize - (inventory[i] as ItemStack).size);
					(inventory[i] as ItemStack).size = newSize;
					if (quantity == 0) {
						EmitSignal (nameof (inventory_change));
						return;
					}
				}
			}
		for (int i = 0; i < inventory.Count; i++)
			if (inventory[i].item == ItemId.NULL) {
				inventory[i] = Item.Builder.MakeSlot (item, (ushort) Mathf.Min (quantity, data.stackSize));
				quantity -= (ushort) Mathf.Min (quantity, data.stackSize);
				if (quantity == 0) {
					EmitSignal (nameof (inventory_change));
					return;
				}
			}
		GD.Print ("Inventory Overflow, the rest of the items were lost");
	}

	public void Remove (ItemSlot slot) { Remove (slot.item, slot.size); }

	public void Remove (ItemId item, ushort quantity = 1) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == item) {
				int newSize = inventory[i].size - quantity;
				if (newSize > 0) {
					(inventory[i] as ItemStack).size -= quantity;
					EmitSignal (nameof (inventory_change));
					return;
				} else {
					inventory[i] = Item.Builder.MakeSlot (ItemId.NULL);
				}
				if (newSize < 0)
					quantity = (ushort) (-newSize);
				else return;
			}
		}
		GD.PrintErr ("[Inventory] Couldn't remove ", quantity, " of ", item.data);
	}

	public bool Contains (ItemSlot slot) { return Contains (slot.item, slot.size); }

	public bool Contains (ItemId item, int quantity = 1) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == item) {
				quantity -= inventory[i].size;
				if (quantity <= 0) return true;
			}
		}
		return false;
	}

	public void MakeCraft (CraftId craftId) {
		CraftData craft = Craft.Manager.GetCraft (craftId);
		foreach (Ingredient ingredient in craft.ingredients) {
			Remove (ingredient.ToItemSlot ());
		};
		Add (Item.Manager.GetId (craft.result), craft.amount);
	}

	public bool CanCraft (CraftId craft) {
		foreach (Ingredient ingredient in Craft.Manager.GetCraft (craft).ingredients) {
			if (!Contains (ingredient.ToItemSlot ()))
				return false;
		};
		return true;
	}

	public void Use (ItemId item) {
		if (item == ItemId.NULL)
			return;
		if (!Contains (item)) {
			GD.Print ("You don't have this item :", item);
			return;
		} else if (item.category == Item.Manager.GetCategory ("equipement").id) {
			Remove (item);
			GetNode<_GUIItem> ("/root/GUI/EquipedItem").Display (Item.Builder.MakeSlot (item));
		} else if (item.category == Item.Manager.GetCategory ("food").id) {
			Remove (item);
			GD.Print ("Miam, it's delicious!");
		}
	}

	public Godot.Collections.Dictionary<string, object> MakeSave () {
		var saveObject = new Godot.Collections.Dictionary<string, object> () { { "Path", GetPath () } };
		for (int i = 0; i < INVENTORY_SIZE; i++)
			saveObject["Item" + i.ToString ()] = inventory[i].Serialize ();
		return saveObject;
	}
	public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		for (int i = 0; i < INVENTORY_SIZE; i++)
			inventory[i] = Builder.DeserializeSlot (saveObject["Item" + i.ToString ()].ToString ());
		EmitSignal (nameof (inventory_change));
	}
}