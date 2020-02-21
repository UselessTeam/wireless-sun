using System;
using Godot;

public class Inventory : Node2D {
    ItemStack[] inventory;
    public override void _Ready () {
        // Item test = new Item (ItemType.Food, (int) Food.berry);
        // GD.Print (test.ToString ());
    }

}