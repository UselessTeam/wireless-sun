using System;
using Godot;
using Newtonsoft.Json;

namespace Item {
    public static class EquipmentManager {

        public static void LoadWeapons () {
            var file = new File ();
            file.Open ("res://Data/weapons.json", File.ModeFlags.Read);
            var equipements = new ItemCategory ();
            equipements.name = "equipement";
            equipements.stackSize = 1;
            equipements.items = JsonConvert.DeserializeObject<WeaponData[]> (file.GetAsText ()); //new List<ItemData> ();
            file.Close ();
            Manager.categories.Add (equipements);
        }
    }

    public class EquipementData : ItemData {
        [Export] public float armor = 0;
        [Export] public string type = "none";
        [Export] public string location = "hand";
    }

    public class WeaponData : EquipementData {
        [Export] public string action = "attack";
        [Export] public float damage = 0;
        [Export] public float range = 1;
        [Export] public string[] effects = new string[0];

        public WeaponData (float armor = 0, float damage = 20, float range = 0.4f) {
            this.armor = armor;
            this.damage = damage;
            this.range = range;
        }

        public static WeaponData EmptyHand = new WeaponData ();
    }

}