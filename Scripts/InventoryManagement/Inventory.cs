using System;
using Godot;

public class Inventory : Node2D {
    ItemStack[] inventory;
    public override void _Ready () {
        GD.Print (ItemManager.GetItemByName ("Berry"));
        GD.Print (ItemManager.GetItemByName ("ItemQuiExistePas"));

    }

}