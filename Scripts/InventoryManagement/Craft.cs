using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Craft {
    public static class Manager {
        public static string dataPath = "res://Data/Crafts";

        private static List<CraftLocation> locations = new List<CraftLocation> ();
        private static Dictionary<string, byte> locationNames = new Dictionary<string, byte> ();

        public static void Load () {
            var craftsDir = new Directory ();
            craftsDir.Open (dataPath);
            craftsDir.ListDirBegin (true, true);
            string locationName;
            locationName = craftsDir.GetNext ();
            byte locationId = 0;
            while (locationName != "") {
                var locationDir = new Directory ();
                locationDir.Open (dataPath.PlusFile (locationName));
                locationDir.ListDirBegin (true, true);
                locations.Add (new CraftLocation (locationName, locationId));
                locationNames[locationName.ToLower ()] = locationId;
                byte craftId = 0;
                string craftName;
                do {
                    craftName = locationDir.GetNext ();
                    if (craftName.EndsWith (".tres")) {
                        var loadedResource = (GD.Load (dataPath.PlusFile (locationName).PlusFile (craftName)) as CraftResource);
                        loadedResource.id = new CraftId (locationId, craftId);
                        loadedResource.result = craftName.Remove (craftName.Length - ".tres".Length);
                        locations[locationId].crafts.Add (loadedResource);
                    }
                    craftId += 1;
                } while (craftName != "");
                locationDir.ListDirEnd ();
                locationName = craftsDir.GetNext ();
                locationId += 1;
            }

        }

        public static CraftLocation GetLocationById (byte locationId) {
            return locations[locationId];
        }

        public static CraftResource GetCraft (CraftId id) {
            return GetLocationById (id.location).crafts[id.craft];
        }

        public static CraftLocation GetCraftLocation (string name) {
            byte id;
            if (name != null && locationNames.TryGetValue (name.ToLower (), out id))
                return GetLocationById (id);
            GD.PrintErr ("Error: Trying to get a non existing craft location : " + name);
            return CraftLocation.NULL; // TODO: Once Godot is updated (and fixes the GD.Print(null) crash) we can return null here
        }
    }

    public class CraftLocation {
        public byte id = 0;

        public string name;
        public List<CraftResource> crafts = new List<CraftResource> ();

        public CraftLocation (string name, byte id = 0) {
            this.id = id;
            this.name = name;
        }

        public static readonly CraftLocation NULL = new CraftLocation ("NULL", byte.MaxValue);
    }

    public struct CraftId {
        public byte location;
        public byte craft;

        public static readonly CraftId NULL = new CraftId (byte.MaxValue, byte.MaxValue);
        public CraftId (byte location, byte craft) {
            this.location = location;
            this.craft = craft;
        }

        public CraftResource data { get { return Manager.GetCraft (this); } }

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

    public class Ingredient {
        public string item;
        public ushort amount;
        public Item.ItemSlot ToItemSlot () {
            return Item.Builder.MakeSlot (Item.Manager.GetId (item), amount);
        }
    }
}