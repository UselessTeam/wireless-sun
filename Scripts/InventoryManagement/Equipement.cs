using System.Collections.Generic;
using Godot;
using Item;
using Newtonsoft.Json;

public class EquipementManager {
    public ItemSlot rightHand = Builder.MakeSlot (ItemId.NULL);
    public ItemSlot leftHand = Builder.MakeSlot (ItemId.NULL);
    public ItemSlot helmet = Builder.MakeSlot (ItemId.NULL);
    public ItemSlot torso = Builder.MakeSlot (ItemId.NULL);
    public ItemSlot boots = Builder.MakeSlot (ItemId.NULL);

    public void Equip (ItemSlot item) {
        GameRoot.inventory.Add (rightHand.item);
        rightHand = item;
    }

    public float GetDamage () {
        if (rightHand.item == ItemId.NULL)
            return 20;
        return Manager.GetItem<EquipementData> (rightHand.item).damage;
    }
    public float GetRange () {
        if (rightHand.item == ItemId.NULL)
            return 0.5f;
        return Manager.GetItem<EquipementData> (rightHand.item).range;
    }
    // public float GetType () { return Manager.GetItem<EquipementData> (rightHand.item).type; }

    public string Serialize () {
        return JSON.Print (
            new Dictionary<string, object> () {
                {
                    "rightHand",
                    rightHand.Serialize ()
                }, {
                    "leftHand",
                    leftHand.Serialize ()
                }, {
                    "helmet",
                    helmet.Serialize ()
                }, {
                    "torso",
                    torso.Serialize ()
                }, {
                    "boots",
                    boots.Serialize ()
                }
            }
        );
    }
    public void Deserialize (string serializedData) {
        var equipementData = JsonConvert.DeserializeObject<Dictionary<string, object>> (serializedData);
        rightHand = Builder.DeserializeSlot (equipementData["rightHand"].ToString ());
        leftHand = Builder.DeserializeSlot (equipementData["leftHand"].ToString ());
        helmet = Builder.DeserializeSlot (equipementData["helmet"].ToString ());
        torso = Builder.DeserializeSlot (equipementData["torso"].ToString ());
        boots = Builder.DeserializeSlot (equipementData["boots"].ToString ());
    }
}