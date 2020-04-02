using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Craft {
    public static class Manager {
        private static CraftLocation[] locations;
        private static Dictionary<string, byte> locationNames = new Dictionary<string, byte> ();

        public static void Load () {
            Godot.File file = new Godot.File ();
            file.Open ("res://Data/crafts.json", Godot.File.ModeFlags.Read);
            locations = JsonConvert.DeserializeObject<CraftLocation[]> (file.GetAsText ());
            byte locationId = 0;
            foreach (CraftLocation location in locations) {
                location.id = locationId;
                byte craftId = 0;
                foreach (CraftData craft in location.crafts) {
                    craft.id = new CraftId (locationId, craftId);
                    craftId += 1;
                }
                locationNames[location.name] = location.id;
                locationId += 1;
            }
        }

        public static CraftLocation GetLocationById (byte locationId) {
            return locations[locationId];
        }

        public static CraftData GetCraft (CraftId id) {
            return GetLocationById (id.location).crafts[id.craft];
        }

        public static CraftLocation GetCraftLocation (string name) {
            byte id;
            if (name != null && locationNames.TryGetValue (name.ToLower (), out id))
                return GetLocationById (id);
            GD.PrintErr ("Error: Trying to get a non existing item : " + name);
            return CraftLocation.NULL; // TODO: Once Godot is updated (and fixes the GD.Print(null) crash) we can return null here
        }
    }

    public class CraftLocation {
        public byte id = 0;

        [JsonProperty ("location")] public string name;
        public CraftData[] crafts;

        public CraftLocation (byte id, string name) {
            this.id = id;
            this.name = name;
            this.crafts = new CraftData[0];
        }

        public static readonly CraftLocation NULL = new CraftLocation (byte.MaxValue, "NULL");
    }

    public struct CraftId {
        public byte location;
        public byte craft;

        public static readonly CraftId NULL = new CraftId (byte.MaxValue, byte.MaxValue);
        public CraftId (byte location, byte craft) {
            this.location = location;
            this.craft = craft;
        }

        public CraftData data { get { return Manager.GetCraft (this); } }

        // Overrides

        public override bool Equals (object obj) {
            if (!(obj is CraftId)) {
                return false;
            }
            CraftId other = (CraftId) obj;
            return this.location.Equals (other.location) && this.craft.Equals (other.craft);
        }

        public override int GetHashCode () {
            return this.location.GetHashCode () + 65565 * this.craft.GetHashCode ();
        }

        public static bool operator == (CraftId a, CraftId b) {
            return a.Equals (b);
        }

        public static bool operator != (CraftId a, CraftId b) {
            return !a.Equals (b);
        }
    }

    public class CraftData {
        public CraftId id;
        public CraftLocation location { get { return Manager.GetLocationById (id.location); } }

        public string result;
        public ushort amount = 1;
        public Ingredient[] ingredients;

        public CraftData () {
            this.id = CraftId.NULL;
            this.result = null;
        }

        public override string ToString () {
            if (this == NULL) {
                return "NULL ITEM";
            }
            return result + " is craftable at " + location + " (id=" + id.ToString () + ")";
        }

        public Item.ItemStack ToItemStack () {
            return new Item.ItemStack (Item.Manager.GetId (result), amount);
        }

        public static readonly CraftData NULL = new CraftData ();
    }

    public class Ingredient {
        public string item;
        public ushort amount;
        public Item.ItemStack ToItemStack () {
            return new Item.ItemStack (Item.Manager.GetId (item), amount);
        }
    }
}