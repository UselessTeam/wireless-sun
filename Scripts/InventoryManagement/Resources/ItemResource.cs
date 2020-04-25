using System;
using Godot;
using Item;

public class ItemResource : Resource {
	public ItemId id;

	public string name = "";
	[Export] public ushort stackSize = 12;
	[Export] public Texture icon;

	public ItemResource () {
		this.id = ItemId.NULL;
		this.name = null;
	}

	public override string ToString () {
		if (this == NULL) {
			return "NULL ITEM";
		}
		return name + " (id=" + id.ToString () + ")";
	}

	public static readonly ItemResource NULL = new ItemResource ();
}