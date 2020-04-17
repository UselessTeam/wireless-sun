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
	public EquipementManager equipement = new EquipementManager ();

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
		EmitSignal (nameof (inventory_change));
	}

	public void Add (ItemId item, ushort quantity = 1) {
		if (item == ItemId.NULL)
			return;
		ItemData data = Item.Manager.GetItem (item);
		if (data.stackSize > 1)
			for (byte i = 0; i < INVENTORY_SIZE; i++) {
				if (inventory[i].item == item) {
					ushort newSize = (ushort) Mathf.Min ((inventory[i].slot as ItemStack).size + quantity, data.stackSize);
					quantity -= (ushort) (newSize - (inventory[i].slot as ItemStack).size);
					(inventory[i].slot as ItemStack).size = newSize;
					if (quantity == 0) {
						EmitSignal (nameof (inventory_change));
						return;
					}
				}
			}
		for (byte i = 0; i < INVENTORY_SIZE; i++)
			if (inventory[i].item == ItemId.NULL) {
				inventory[i].slot = Item.Builder.MakeSlot (item, (ushort) Mathf.Min (quantity, data.stackSize));
				quantity -= (ushort) Mathf.Min (quantity, data.stackSize);
				if (quantity == 0) {
					EmitSignal (nameof (inventory_change));
					return;
				}
			}
		GD.Print ("Inventory Overflow, the rest of the items were lost");
	}

	public void Remove (byte index) {
		inventory[index].slot = Item.Builder.MakeSlot (ItemId.NULL);
		EmitSignal (nameof (inventory_change));
	}

	public void Remove (InventorySlot slot) {
		Remove (slot.index);
	}

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

	public void Use (byte slotIndex) {
		Use (inventory[slotIndex]);
	}

	public void Use (InventorySlot slot) {
		if (slot.item == ItemId.NULL)
			return;
		if (slot.index >= INVENTORY_SIZE) {
			GD.Print ("You don't have this item :", slot.item);
			return;
		} else if (slot.item.category == Item.Manager.GetCategory ("equipement").id) {
			Remove (slot.index);
			equipement.Equip (slot.slot);
			EmitSignal (nameof (equipement_change));
		} else if (slot.item.category == Item.Manager.GetCategory ("food").id) {
			Remove (slot.item, 1);
			GD.Print ("Miam, it's delicious!");
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
	}
}