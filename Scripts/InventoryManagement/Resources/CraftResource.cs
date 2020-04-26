using System;
using Craft;
using Godot;

public class CraftResource : Resource {
    public CraftId id;
    public Craft.CraftLocation location { get { return Craft.Manager.GetLocationById (id.location); } }

    public string result;
    [Export] public ushort amount = 1;
    [Export] private string[] ingredients;
    [Export] private int[] amounts;

    public Item.ItemSlot[] IngredientsSlot {
        get {
            var returnVal = new Item.ItemSlot[ingredients.Length];
            for (var i = 0; i < ingredients.Length; i++)
                returnVal[i] = Item.Builder.MakeSlot (Item.Manager.GetId (ingredients[i]), (ushort) amounts[i]);
            return returnVal;
        }
    }

    public CraftResource () {
        this.id = CraftId.NULL;
        this.result = null;
    }

    public override string ToString () {
        if (this == NULL) {
            return "NULL ITEM";
        }
        return result + " is craftable at " + location + " (id=" + id.ToString () + ")";
    }

    public Item.ItemSlot ToItemSlot () {
        return Item.Builder.MakeSlot (Item.Manager.GetId (result), amount);
    }

    public static readonly CraftResource NULL = new CraftResource ();
}