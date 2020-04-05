using System;
using Godot;

public class CraftableItem : SelectableItem {
	Craft.CraftData craft;
	public void Display (Craft.CraftData craft) {
		base.Display (new Item.ItemStack (craft.result, craft.amount));
		this.craft = craft;

	}

	// Action to perform when double click
	public override void DoubleClick () {
		if (GameRoot.inventory.CanCraft (craft.id))
			GameRoot.inventory.MakeCraft (craft.id);
		else GD.Print ("Cannot craft");
	}
}