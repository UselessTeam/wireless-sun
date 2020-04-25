using System.Collections.Generic;
using Godot;
using Item;
using Newtonsoft.Json;

public struct EquipementSlot {
    public ItemSlot slot;
    public bool rightLeft;
    public ItemId item { get { return slot.item; } }
    public EquipementSlot (ItemSlot slot, bool rightLeft = true) { this.slot = slot; this.rightLeft = rightLeft; }
}

public class EquipementManager {
    public Dictionary<string, ItemSlot> EquipementList = new Dictionary<string, ItemSlot> () {
        {
        "RightHand",
        Builder.MakeSlot (ItemId.NULL)
        }, {
        "LeftHand",
        Builder.MakeSlot (ItemId.NULL)
        }, {
        "Helmet",
        Builder.MakeSlot (ItemId.NULL)
        }, {
        "Torso",
        Builder.MakeSlot (ItemId.NULL)
        }, {
        "Boots",
        Builder.MakeSlot (ItemId.NULL)
        }
    };

    public bool Equip (ItemSlot item) {
        if (!(item.item.data is EquipementData))
            return false;
        GameRoot.inventory.Add (EquipementList["RightHand"].item);
        EquipementList["RightHand"] = item;
        GameRoot.inventory.EmitSignal ("equipement_change");
        return true;
    }
    public bool EquipAs (ItemSlot item, string equipementType) {
        return Equip (item);
    }

    public void UnequipAndStore (string name) {
        if (GameRoot.inventory.Add (EquipementList[name].item) == 0)
            Unequip (name);
    }
    public void Unequip (string name) {
        EquipementList[name] = Builder.MakeSlot (ItemId.NULL);
        GameRoot.inventory.EmitSignal ("equipement_change");
    }

    public float GetDamage () {
        if (EquipementList["RightHand"].item == ItemId.NULL)
            return 20;
        return Manager.GetItem<EquipementData> (EquipementList["RightHand"].item).damage;
    }
    public float GetRange () {
        if (EquipementList["RightHand"].item == ItemId.NULL)
            return 0.5f;
        return Manager.GetItem<EquipementData> (EquipementList["RightHand"].item).range;
    }
    // public float GetType () { return Manager.GetItem<EquipementData> (rightHand.item).type; }

    public string Serialize () {
        var equipementData = new Dictionary<string, object> ();
        foreach (var equipment in EquipementList) {
            equipementData[equipment.Key] = equipment.Value.Serialize ();
        }
        return JSON.Print (equipementData);
    }
    public void Deserialize (string serializedData) {
        var equipementData = JsonConvert.DeserializeObject<Dictionary<string, object>> (serializedData);
        foreach (var equipment in equipementData) {
            EquipementList[equipment.Key] = Builder.DeserializeSlot (equipment.Value.ToString ());
        }
    }
}