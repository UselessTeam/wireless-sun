using System.Collections.Generic;
using Craft;
using Godot;
using Item;

public struct InventorySlot {
	public ItemSlot slot;
	public byte index;
	public ItemId item { get { return slot.item; } }
	public ushort size { get { return slot.size; } }
	public InventorySlot (ItemSlot slot, byte position = byte.MaxValue) { this.slot = slot; this.index = position; }
}

public class Inventory : Node2D {
	const ushort INVENTORY_SIZE = 24;
	public InventorySlot[] inventory = new InventorySlot[INVENTORY_SIZE];
	public EquipementHolder equipement;

	[Signal] public delegate void inventory_change ();
	[Signal] public delegate void equipement_change ();

	public override void _Ready () {
		AddToGroup ("SaveNodes");
		GameRoot.inventory = this;
		InitializeEmpty ();
	}
	public void InitializeEmpty () {
		for (byte i = 0; i < INVENTORY_SIZE; i++)
			inventory[i] = (new InventorySlot (new EmptySlot (), i));
		equipement = new EquipementHolder ();
		EmitSignal (nameof (inventory_change));
		EmitSignal (nameof (equipement_change));
	}

	//
	// Adds the item at the specied position
	// If the inventory slot cannot fit the items, it will still the maximum number of items that it can fit.
	// Returns the number of items that wouldn't fit
	public ushort Add (byte index, ItemId item, ushort quantity = 1) {
		if (inventory[index].item == item && item.data.stackSize > 1) {
			ushort newSize = (ushort) Mathf.Min ((inventory[index].slot as ItemStack).size + quantity, item.data.stackSize);
			quantity -= (ushort) (newSize - (inventory[index].slot as ItemStack).size);
			(inventory[index].slot as ItemStack).size = newSize;
		} else if (inventory[index].item == ItemId.NULL) {
			inventory[index].slot = Item.Builder.MakeSlot (item, (ushort) Mathf.Min (quantity, item.data.stackSize));
			quantity -= (ushort) Mathf.Min (quantity, item.data.stackSize);

		}
		EmitSignal (nameof (inventory_change));
		return quantity;
	}

	//
	// Adds the item in the inventory
	// If the inventory cannot fit the items, it will still the maximum number of items that it can fit.
	// Returns the number of items that wouldn't fit
	public ushort Add (ItemId item, ushort quantity = 1) {
		if (item == ItemId.NULL)
			return 0;
		if (item.data.stackSize > 1)
			for (byte i = 0; i < INVENTORY_SIZE && quantity > 0; i++) {
				if (inventory[i].item == item) {
					quantity = Add (i, item, quantity);
				}
			}
		for (byte i = 0; i < INVENTORY_SIZE && quantity > 0; i++)
			quantity = Add (i, item, quantity);
		return quantity;
	}

	//
	// Adds the item at a specified position
	// If the specified position is not empty, it will throw an error
	public void AddSlot (byte index, ItemSlot slot) {
		if (!inventory[index].item.IsNull ())
			GD.PrintErr ("Trying to AddSlot on an occupied inventory slot");
		else
			inventory[index].slot = slot;
	}

	//
	// Removes a certain amount of item at the specified position
	// If there is not enough items to be removed, it will simply remove all the items
	public void Remove (byte index, ushort quantity = 1) {
		if (quantity == 0)
			return;
		if (inventory[index].slot.size > quantity)
			(inventory[index].slot as ItemStack).size -= quantity;
		else
			RemoveSlot (index);
		EmitSignal (nameof (inventory_change));
	}

	//
	// Removes a certain amount of item at the specified inventory slot
	// If there is not enough items to be removed, it will simply remove all the items
	public void Remove (InventorySlot slot, ushort quantity = 1) {
		Remove (slot.index, quantity);
	}

	//
	// Removes the specified items
	// If there is not enough items to be removed, it will throw an error
	public void Remove (ItemSlot slot) { Remove (slot.item, slot.size); }
	public void Remove (ItemId item, ushort quantity = 1) {
		for (byte i = 0; i < INVENTORY_SIZE; i++) {
			if (inventory[i].item == item) {
				int newSize = inventory[i].size - quantity;
				if (newSize > 0) {
					(inventory[i].slot as ItemStack).size -= quantity;
				} else {
					inventory[i].slot = Item.Builder.MakeSlot (ItemId.NULL);
				}
				EmitSignal (nameof (inventory_change));
				if (newSize < 0)
					quantity = (ushort) (-newSize);
				else return;
			}
		}
		GD.PrintErr ("[Inventory] Couldn't remove ", quantity, " of ", item.data);
	}

	//
	// Removes all items at the speciefied position
	public void RemoveSlot (byte index) {
		inventory[index].slot = Item.Builder.MakeSlot (ItemId.NULL);
		EmitSignal (nameof (inventory_change));
	}

	//
	// Removes all items at the speciefied inventory slot
	public void RemoveSlot (InventorySlot slot) {
		Remove (slot.index);
	}

	public void SwapSlots (byte index1, byte index2) {
		if (index1 == index2) return;
		if (inventory[index1].item == inventory[index2].item && inventory[index1].item.data.stackSize > 1) {
			(inventory[index2].slot as ItemStack).size = Add (index1, inventory[index1].item, inventory[index2].size);
			if (inventory[index2].slot.size == 0)
				RemoveSlot (index2);
		} else {
			var keep = inventory[index1].slot;
			inventory[index1].slot = inventory[index2].slot;
			inventory[index2].slot = keep;
		}
		EmitSignal (nameof (inventory_change));
	}

	public bool Contains (ItemSlot slot) { return Contains (slot.item, slot.size); }

	public bool Contains (ItemId item, int quantity = 1) {
		for (byte i = 0; i < INVENTORY_SIZE; i++) {
			if (inventory[i].item == item) {
				quantity -= inventory[i].size;
				if (quantity <= 0) return true;
			}
		}
		return false;
	}

	public void MakeCraft (CraftId craftId) {
		CraftResource craft = Craft.Manager.GetCraft (craftId);
		foreach (var ingredient in craft.IngredientsSlot) {
			Remove (ingredient);
		};
		Add (Item.Manager.GetId (craft.result), craft.amount);
	}

	public bool CanCraft (CraftId craft) {
		foreach (var ingredient in Craft.Manager.GetCraft (craft).IngredientsSlot) {
			if (!Contains (ingredient))
				return false;
		};
		return true;
	}

	public void Use (byte slotIndex) {
		Use (inventory[slotIndex]);
	}

	public void Use (InventorySlot slot) {
		if (slot.item == ItemId.NULL)
			return;
		if (slot.index >= INVENTORY_SIZE) {
			GD.Print ("You don't have this item :", slot.item);
			return;
		} else if (slot.slot is EquipementSlot) {
			RemoveSlot (slot);
			equipement.Equip (slot.slot as EquipementSlot);
		} else if (slot.item.data is FoodResource) {
			Remove (slot, 1);
			Gameplay.myPlayer.GetNode<Health> ("PlayerControl/Health").HP += 10;

		}
	}

	public Godot.Collections.Dictionary<string, object> MakeSave () {
		var saveObject = new Godot.Collections.Dictionary<string, object> () { { "Path", GetPath () } };
		for (int i = 0; i < INVENTORY_SIZE; i++)
			saveObject["Item" + i.ToString ()] = inventory[i].slot.Serialize ();
		saveObject["Equipement"] = equipement.Serialize ();
		return saveObject;
	}
	public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		for (int i = 0; i < INVENTORY_SIZE; i++)
			inventory[i].slot = Builder.DeserializeSlot (saveObject["Item" + i.ToString ()].ToString ());
		equipement.Deserialize (saveObject["Equipement"].ToString ());
		EmitSignal (nameof (inventory_change));
		EmitSignal (nameof (equipement_change));
	}
}