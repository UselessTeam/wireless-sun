using System;
using System.Globalization;
using Godot;
using Item;

public class ItemResource : Resource {
	public ItemId id;

	public string name = "";
	[Export] public ushort stackSize = 12;
	[Export] public Texture icon;

	public static TextInfo myTI = new CultureInfo ("en-US", false).TextInfo;
	public string DisplayName () { return myTI.ToTitleCase (name.Replace ('_', ' ')); }

	public ItemResource () {
		this.id = ItemId.NULL;
		this.name = null;
	}

	public override string ToString () {
		if (this == NULL) {
			return "NULL ITEM";
		}
		return DisplayName () + " (id=" + id.ToString () + ")";
	}

	public static readonly ItemResource NULL = new ItemResource ();
}