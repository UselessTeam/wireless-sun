using System;
using Godot;

public class InventoryItem : MovableItem {
    // Action to perform when double click

    byte index = byte.MaxValue;

    public override void _Ready () {
        base._Ready ();
    }

    public override void DoubleClick () {
        GameRoot.inventory.Use (index);
    }
    public void Display (InventorySlot slot) {
        index = slot.index;
        Display (slot.slot);
    }

    public override void MoveIn (SelectableItem item) {
        if (item is InventoryItem) {
            GameRoot.inventory.SwapSlots (index, (item as InventoryItem).index);
        }
        if (item is EquipementItem) {
            if (MySlot.item.IsNull ()) {
                GameRoot.inventory.AddSlot (index, item.MySlot);
                GameRoot.inventory.equipement.Unequip (item.Name);
            } else GameRoot.inventory.equipement.UnequipAndStore (item.Name);
        }
    }

    public void RemoveMe () {
        GameRoot.inventory.RemoveSlot (index);
    }

}