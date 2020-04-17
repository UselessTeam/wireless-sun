using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Item {
	public static class Manager {
		// private static ItemCategory[] categories;
		private static List<ItemCategory> categories; // = new List<ItemCategory> ();

		private static Dictionary<string, ItemId> itemNames = new Dictionary<string, ItemId> ();
		public static void Load () {
			// var saveGame = new File ();
			// saveGame.Open ("Data/items2.json", File.ModeFlags.Read);
			// while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// 	var data = JsonConvert.DeserializeObject<Dictionary<string, object>> (saveGame.GetLine ());
			// 	if (data.ContainsKey ("category"))
			// 		categories2.Add (new ItemCategory ());
			// }

			Godot.File file = new Godot.File ();
			file.Open ("res://Data/items.json", Godot.File.ModeFlags.Read);
			categories = JsonConvert.DeserializeObject<List<ItemCategory>> (file.GetAsText ());
			byte categoryId = 0;
			file.Close ();

			LoadEquipements ();

			foreach (ItemCategory category in categories) {
				category.id = categoryId;
				byte itemId = 0;
				foreach (ItemData item in category.items) {
					item.id = new ItemId (categoryId, itemId);
					itemNames[item.name] = item.id;
					itemId += 1;
				}
				categoryId += 1;
			}
		}

		public static void LoadEquipements () {
			var file = new File ();
			file.Open ("res://Data/equipement.json", File.ModeFlags.Read);
			var equipements = new ItemCategory ();
			equipements.name = "equipement";
			equipements.stackSize = 1;
			equipements.items = JsonConvert.DeserializeObject<EquipementData[]> (file.GetAsText ()); //new List<ItemData> ();
			file.Close ();
			categories.Add (equipements);
		}

		public static ItemCategory GetCategory (byte categoryId) {
			return categories[categoryId];
		}
		public static ItemCategory GetCategory (string categoryName) {
			foreach (var category in categories) {
				if (category.name == categoryName)
					return category;
			}
			GD.Print ("This category does not exist :", categoryName);
			return null;
		}

		public static ItemData GetItem (ItemId id) {
			return GetItem<ItemData> (id);
		}
		public static ItemData GetItem (string name) {
			return GetItem<ItemData> (name);
		}
		public static T GetItem<T> (ItemId id) where T : ItemData {
			return (T) GetCategory (id.category).items[id.item];
		}
		public static T GetItem<T> (string name) where T : ItemData {
			ItemId id;
			if (name != null && itemNames.TryGetValue (name.ToLower (), out id))
				return (T) GetItem<T> (id);
			GD.PrintErr ("Error: Trying to get a non existing item : " + name);
			return (T) ItemData.NULL; // TODO: Once Godot is updated (and fixes the GD.Print(null) crash) we can return null here
		}

		public static ItemId GetId (string name) {
			ItemId id;
			if (name != null && itemNames.TryGetValue (name.ToLower (), out id))
				return id;
			GD.PrintErr ("Error: Trying to get a non existing item : " + name);
			return ItemId.NULL;
		}
	}

	public class ItemCategory {
		public byte id = 0;

		[JsonProperty ("category")]
		public string name;
		public ItemData[] items;
		public ushort stackSize;
	}

	public struct ItemId {
		public byte category;
		public byte item;

		public static readonly ItemId NULL = new ItemId (byte.MaxValue, byte.MaxValue);
		public ItemId (byte category, byte item) {
			this.category = category;
			this.item = item;
		}

		public ItemData data { get { return Manager.GetItem (this); } }

		// Overrides

		public override bool Equals (object obj) {
			if (!(obj is ItemId)) {
				return false;
			}

			ItemId other = (ItemId) obj;

			return this.category.Equals (other.category) && this.item.Equals (other.item);
		}

		public override int GetHashCode () {
			return this.category.GetHashCode () + 65565 * this.item.GetHashCode ();
		}

		public static bool operator == (ItemId a, ItemId b) {
			return a.Equals (b);
		}

		public static bool operator != (ItemId a, ItemId b) {
			return !a.Equals (b);
		}

		public override string ToString () {
			if (this == NULL) return "";
			return Manager.GetItem (this).name;
		}
	}

	public class ItemData {
		public ItemId id;
		public ItemCategory category { get { return Manager.GetCategory (id.category); } }

		public string name;
		public ushort stackSize { get { return category.stackSize; } }

		public Graphics.Sprite sprite;

		public ItemData () {
			this.id = ItemId.NULL;
			this.name = null;
		}

		public override string ToString () {
			if (this == NULL) {
				return "NULL ITEM";
			}
			return name + " of type " + category + " (id=" + id.ToString () + ")";
		}

		public static readonly ItemData NULL = new ItemData ();
	}
	public class EquipementData : ItemData {
		public float damage = 0;
		public float range = 1;
		public string type = "none";
	}

	public static class ItemExtensions {
		public static string ToPrint (this IEnumerable<ItemStack> stacks) {
			return "[" + string.Join (", ", stacks) + "]";
		}
	}
}
