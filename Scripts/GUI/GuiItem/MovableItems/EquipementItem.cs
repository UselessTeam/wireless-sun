using System;
using Godot;

public class EquipementItem : MovableItem {
    [Export] Texture emptyEquipementTexture;

    Sprite EmptyEquipementSprite { get { return GetNode<Sprite> ("EmptyEquipement"); } }

    public override void _Ready () {
        base._Ready ();
        EmptyEquipementSprite.Texture = emptyEquipementTexture;
        // this.Connect ("gui_input", this, "_on_EquipementItem_gui_input");
    }
    public override void Display (Item.ItemSlot item) {
        base.Display (item);
        if (item.item == Item.ItemId.NULL)
            EmptyEquipementSprite.Show ();
        else
            EmptyEquipementSprite.Hide ();
    }

    public override void DoubleClick () {
        GameRoot.inventory.equipement.UnequipAndStore (Name);
    }

    public override void MoveIn (SelectableItem item) {
        if (item is InventoryItem) {
            if (GameRoot.inventory.equipement.EquipAs (item.MySlot, Name))
                (item as InventoryItem).RemoveMe ();

        }
        if (item is EquipementItem) {
            if (GameRoot.inventory.equipement.EquipAs (item.MySlot, Name)) {
                (item as InventoryItem).RemoveMe ();
            }
        }
    }
    // public void _on_EquipementItem_gui_input (InputEvent _event) {}

}