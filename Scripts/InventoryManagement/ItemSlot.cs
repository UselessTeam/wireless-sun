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
            var data = item.data;
            if (data is FoodResource)
                return data.DisplayName () + " (" + size + ")\n" +
                    "+ " + (data as FoodResource).hpRecovery + " HP";
            else //Item Resource
                return item.data.DisplayName () + " (" + size + ")";
        }
    }

    public class UniqueItem : ItemSlot {
        public UniqueItem (ItemId item) { this.item = item; }
        public override string GetLabel () { return ""; }
        public override string ToString () {
            return item.data.DisplayName ();
        }
    }
    public class EquipementSlot : UniqueItem {
        public EquipementSlot (ItemId item) : base (item) {}
        public EquipementResource equipementData { get { return Manager.GetItem<EquipementResource> (item); } }
        public override string ToString () {
            if (equipementData is WeaponResource) {
                var data = (equipementData as WeaponResource);
                var returnString = item.data.DisplayName () + "\n";
                if (data.Action == ActionType.Block)
                    returnString += "Can block incomming attacks\n";
                if (data.Damage > 0)
                    returnString += "Damage " + data.Damage.ToString () + "\n";
                if (data.Range != 1)
                    returnString += "Range x" + data.Range.ToString () + "\n";
                if (data.Cooldown != 1)
                    returnString += "Cooldown " + data.Cooldown.ToString () + "\n";
                foreach (AttackEffect effect in Enum.GetValues (typeof (AttackEffect)))
                    if ((effect & data.Effects) > 0)
                        returnString += "Can cause " + effect.ToString () + "\n";
                if (data.armor != 0)
                    returnString += "+" + data.armor + " armor\n";
                return returnString.Remove (returnString.Length - "\n".Length);
            } else //Equipement Resource
                return item.data.DisplayName () + "\n" + equipementData.armor + " armor";

        }
    }

    public static class Builder {
        public static string baseItemPath = "res://Nodes/Bodies/Item.tscn";

        public static ItemSlot MakeSlot (ItemId id, ushort quantity = 1) {
            if (id == ItemId.NULL)
                return new EmptySlot ();
            var data = id.data;
            if (data is EquipementResource)
                return new EquipementSlot (id);
            // if (data.stackSize == 1)
            //     return new UniqueItem (id);
            return new ItemStack (id, quantity);
        }

        public static ItemSlot DeserializeSlot (string serializedData) {
            var slotData = JsonConvert.DeserializeObject<Dictionary<string, object>> (serializedData);
            return MakeSlot (new ItemId (Convert.ToUInt16 (slotData["ID"])), Convert.ToUInt16 (slotData["Size"]));
        }

        public static Body MakeBody (string item, ushort quantity = 1) {
            var body = ((PackedScene) GD.Load (baseItemPath)).Instance ().GetNode<Body> ("./");
            body.GetNode<PickableControl> ("./Control").SetStack (item, quantity);
            return body;
        }
    }

}