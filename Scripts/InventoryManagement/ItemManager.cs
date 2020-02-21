using System;
using System.Collections.Generic;
using Godot;

public class ItemManager : Node {
    public static Dictionary<string, int> ItemNumber = new Dictionary<string, int> ();
    // public static Dictionary<string, int> ItemType = new Dictionary<string, int> ();
    public static List<Item> ItemList = new List<Item> ();

    public override void _Ready () {
        int id = 0;
        for (int i = 0; i < GetChildCount (); i++) {
            var Type = GetChild (i);
            for (int j = 0; j < Type.GetChildCount (); j++) {
                var ItemNode = Type.GetChild (j);
                ItemList.Add (new Item (ItemNode.Name, Type.Name, id));
                ItemNumber.Add (ItemNode.Name, i);
                id += 1;
            }
        }
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
        return name + " of type " + type + ". Id : " + id.ToString ();
    }
}

public struct ItemStack {
    Item item;
    int size;
}