using System;
using Godot;

public abstract class IngredientItem : BlandItem {
    public new void Display (Item.ItemStack stack) {
        base.Display (stack);
        if (!GameRoot.inventory.Contains (stack)) {
            this.Modulate = new Color (1, .5f, .5f);
        }

    }
}