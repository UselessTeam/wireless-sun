using System;
using Godot;
using Item;

public class Inventory : Node2D {
	ItemStack[] inventory;
	public override void _Ready () {
		GD.Print (Item.Manager.GetItem ("Berry"));
		GD.Print (Item.Manager.GetItem ("ItemQuiExistePas"));

	}

}
