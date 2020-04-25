using System;
using Godot;

public abstract class IngredientItem : HoverableItem {
    public override void _Ready () {
        GameRoot.inventory.Connect (nameof (Inventory.inventory_change), this, nameof (CheckIngredient));
    }

    public override void Display (Item.ItemSlot slot) {
        base.Display (slot);
        CheckIngredient ();
    }

    public void CheckIngredient () {
        if (!GameRoot.inventory.Contains (MySlot)) {
            this.Modulate = new Color (1, .5f, .5f);
        }
    }
}