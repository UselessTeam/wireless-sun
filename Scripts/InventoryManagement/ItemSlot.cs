using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Item {
    public abstract class ItemSlot {
        public ushort size { get { return ((this as ItemStack) != null) ? (this as ItemStack).size : (ushort) 1; } }
        public ItemId item;

        public abstract string GetLabel ();
        public abstract override string ToString ();
        public virtual string Serialize () {
            return JSON.Print (new Dictionary<string, object> () { { "IDcategory", item.category }, { "IDvariant", item.item }, { "Size", size } });
        }
    }

    public class EmptySlot : ItemSlot {
        public EmptySlot () { item = ItemId.NULL; }
        public override string ToString () { return ""; }
        public override string GetLabel () { return ""; }

    }

    public class ItemStack : ItemSlot {
        public new ushort size;

        public ItemStack (string item, ushort size) { this.item = Manager.GetId (item); this.size = size; }

        public ItemStack (ItemId item, ushort size) {
            this.item = item;
            this.size = size;
        }

        public override string GetLabel () { return size.ToString (); }
        public override string ToString () {
            return item.data.name + " (" + size + ")";
        }
    }

    public class UniqueItem : ItemSlot {
        public UniqueItem (ItemId item) { this.item = item; }
        public override string GetLabel () { return ""; }
        public override string ToString () {
            return item.data.name;
        }
    }
    public class EquipementSlot : UniqueItem {
        public EquipementSlot (ItemId item) : base (item) {}
        public EquipementData equipementData { get { return Manager.GetItem<EquipementData> (item); } }
        public override string ToString () {
            return item.data.name + "\n" +
                equipementData.damage.ToString () + " damage" + "\n" +
                "x" + equipementData.range.ToString () + " range";
        }
    }

    public static class Builder {
        public static string baseItemPath = "res://Nodes/Bodies/Item.tscn";

        public static ItemSlot MakeSlot (ItemId id, ushort quantity = 1) {
            if (id == ItemId.NULL)
                return new EmptySlot ();
            var data = Manager.GetItem (id);
            if (data.category == Manager.GetCategory ("equipement"))
                return new EquipementSlot (id);
            if (data.stackSize == 1)
                return new UniqueItem (id);
            return new ItemStack (id, quantity);
        }

        public static ItemSlot DeserializeSlot (string serializedData) {
            var slotData = JsonConvert.DeserializeObject<Dictionary<string, object>> (serializedData);
            return MakeSlot (new ItemId (Convert.ToByte (slotData["IDcategory"]), Convert.ToByte (slotData["IDvariant"])), Convert.ToUInt16 (slotData["Size"]));
        }

        public static Body MakeBody (string item, ushort quantity = 1) {
            var body = ((PackedScene) GD.Load (baseItemPath)).Instance ().GetNode<Body> ("./");
            body.GetNode<PickableControl> ("./PickableControl").SetStack (item, quantity);
            return body;
        }
    }

}