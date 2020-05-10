using System;
using Godot;

public class CraftableItem : SelectableItem {
    CraftResource craft;
    public void Display (CraftResource craft) {
        base.Display (craft.ToItemSlot ());
        this.craft = craft;

    }

    // Action to perform when double click
    public override void DoubleClick () {
        if (GameRoot.inventory.CanCraft (craft.id))
            GameRoot.inventory.MakeCraft (craft.id);
        else GD.Print ("Cannot craft");
    }
}