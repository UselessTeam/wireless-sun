using System;
using Godot;

public enum Item {
    sword,
    stone,
    wood,
    crableg
}

public class ItemStack {
    Item item;
    int size;
}

public class Inventory : Node2D {
    ItemStack[] inventory;
    public override void _Ready () { }

}