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
            return JSON.Print (new Dictionary<string, object> () { { "ID", item.id }, { "Size", size } });
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
        public EquipementResource equipementData { get { return Manager.GetItem<EquipementResource> (item); } }
        public override string ToString () {
            if (equipementData is WeaponResource) {
                var data = (equipementData as WeaponResource);
                var returnString = item.data.name + "\n";
                if (data.action == ActionType.Block)
                    returnString += "Can block incomming attacks\n";
                if (data.damage > 0)
                    returnString += data.damage.ToString () + " damage" + "\n";
                if (data.range != 1)
                    returnString += "x" + data.range.ToString () + " range" + "\n";
                foreach (var effect in data.effects)
                    returnString += "Can cause " + effect + "\n";
                if (data.armor != 0)
                    returnString += "+" + data.armor + " armor\n";
                return returnString;
            } else
                return item.data.name + "\n" + equipementData.armor + " armor\n";

        }
    }

    public static class Builder {
        public static string baseItemPath = "res://Nodes/Bodies/Item.tscn";

        public static ItemSlot MakeSlot (ItemId id, ushort quantity = 1) {
            if (id == ItemId.NULL)
                return new EmptySlot ();
            var data = Manager.GetItem (id);
            if (data is EquipementResource)
                return new EquipementSlot (id);
            if (data.stackSize == 1)
                return new UniqueItem (id);
            return new ItemStack (id, quantity);
        }

        public static ItemSlot DeserializeSlot (string serializedData) {
            var slotData = JsonConvert.DeserializeObject<Dictionary<string, object>> (serializedData);
            return MakeSlot (new ItemId (Convert.ToUInt16 (slotData["ID"])), Convert.ToUInt16 (slotData["Size"]));
        }

        public static Body MakeBody (string item, ushort quantity = 1) {
            var body = ((PackedScene) GD.Load (baseItemPath)).Instance ().GetNode<Body> ("./");
            body.GetNode<PickableControl> ("./PickableControl").SetStack (item, quantity);
            return body;
        }
    }

}