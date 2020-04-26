using System.Collections.Generic;
using Godot;
using Item;
using Newtonsoft.Json;

public class EquipementHolder {
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
    public static WeaponResource emptyHand = GD.Load ("res://Data/Items/Weapon/empty_hand.tres") as WeaponResource;

    //
    // Tries to equip the given item on an appropriate Equipement slot and to store any previously equiped item in the inventory.
    // If the item is succesfully equiped, returns true
    // If the inventory is full, or if the given item cannot be equiped for any other reason, the function does nothing and returns false
    public bool Equip (EquipementSlot item) {
        var data = (item.item.data as EquipementResource);
        foreach (var equip in EquipementList) {
            if (equip.Key.Contains (data.location.ToString ())) {
                if (data.location == EquipementLocation.Hand && EquipementList["LeftHand"].item.IsNull ())
                    return EquipAs (item, "LeftHand");
                else
                    return EquipAs (item, equip.Key);
            }
        }
        GD.PrintErr ("Error! Couldn't find the location of the equipement : ", data.location);
        return false;
    }
    //
    // Tries to equip the given item on the given Equipement slot and to store any previously equiped item in the inventory.
    // If the item is succesfully equiped, returns true
    // If the inventory is full, or if the given item cannot be equiped for any other reason, the function does nothing and returns false
    public bool EquipAs (EquipementSlot item, string equipLocation) {
        if (!equipLocation.Contains (item.equipementData.location.ToString ()))
            return false;
        if (GameRoot.inventory.Add (EquipementList[equipLocation].item) > 0)
            return false;
        EquipementList[equipLocation] = item;
        GameRoot.inventory.EmitSignal ("equipement_change");
        return true;

    }
    //
    // Unequip the equipement in the given slot and stores it in the inventory.
    // If the inventory is full, nothing happens
    public void UnequipAndStore (string name) {
        if (GameRoot.inventory.Add (EquipementList[name].item) == 0)
            Unequip (name);
    }
    //
    // Unequip and destroy the equipement in the given slot
    public void Unequip (string name) {
        EquipementList[name] = Builder.MakeSlot (ItemId.NULL);
        GameRoot.inventory.EmitSignal ("equipement_change");
    }

    public WeaponResource GetAction (bool isLeft) {
        string location = (isLeft) ? "LeftHand" : "RightHand";
        if (EquipementList[location].item == ItemId.NULL)
            return emptyHand;
        else
            return EquipementList[location].item.data as WeaponResource;
    }

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