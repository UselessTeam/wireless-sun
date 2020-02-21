using System;
using System.Collections.Generic;
using Godot;

public class ItemManager : Node {
    public static Dictionary<string, int> ItemNumber = new Dictionary<string, int> ();
    public static List<Item> ItemList = new List<Item> ();

    public override void _Ready () {
        int id = 0;
        for (int i = 0; i < GetChildCount (); i++) {
            var Type = GetChild (i);
            for (int j = 0; j < Type.GetChildCount (); j++) {
                var ItemNode = Type.GetChild (j);
                ItemList.Add (new Item (ItemNode.Name, Type.Name, id));
                ItemNumber.Add (ItemNode.Name, id);
                id += 1;
            }
        }
    }

    public static Item GetItemByName (string name) {
        int id;
        if (ItemNumber.TryGetValue (name, out id))
            return ItemList[id];
        GD.PrintErr ("Error: Trying to get a non existing item : " + name);
        return new Item (name);
    }

}

public struct Item {
    string name;
    string type;
    int id;
    // TODO: Include sprite resources and stuff

    public Item (string name = "NullName", string type = "NullType", int id = -1) {
        this.name = name;
        this.type = type;
        this.id = id;
    }

    public override string ToString () {
        return name + " of type " + type + " (id=" + id.ToString () + ")";
    }
}

public struct ItemStack {
    Item item;
    int size;
}