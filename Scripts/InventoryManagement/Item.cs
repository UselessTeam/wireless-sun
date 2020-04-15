using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Item {
	public static class Manager {
		private static ItemCategory[] categories;
		private static List<ItemCategory> categories2;

		private static Dictionary<string, ItemId> itemNames = new Dictionary<string, ItemId> ();
		public static void Load () {
			var saveGame = new File ();
			saveGame.Open ("Data/items2.json", File.ModeFlags.Read);
			while (saveGame.GetPosition () < saveGame.GetLen ()) {
				var data = JsonConvert.DeserializeObject<Dictionary<string, object>> (saveGame.GetLine ());
				if (data.ContainsKey ("category"))
					categories2.Add (new ItemCategory ());
			}

			Godot.File file = new Godot.File ();
			file.Open ("res://Data/items.json", Godot.File.ModeFlags.Read);
			categories = JsonConvert.DeserializeObject<ItemCategory[]> (file.GetAsText ());
			byte categoryId = 0;
			foreach (ItemCategory category in categories) {
				category.id = categoryId;
				byte variantId = 0;
				foreach (ItemData item in category.variants) {
					item.id = new ItemId (categoryId, variantId);
					itemNames[item.name] = item.id;
					variantId += 1;
				}
				categoryId += 1;
			}
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
			return GetCategory (id.category).variants[id.variant];
		}

		public static ItemData GetItem (string name) {
			ItemId id;
			if (name != null && itemNames.TryGetValue (name.ToLower (), out id))
				return GetItem (id);
			GD.PrintErr ("Error: Trying to get a non existing item : " + name);
			return ItemData.NULL; // TODO: Once Godot is updated (and fixes the GD.Print(null) crash) we can return null here
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
		public ItemData[] variants;
		public ushort stackSize;
	}

	public struct ItemId {
		public byte category;
		public byte variant;

		public static readonly ItemId NULL = new ItemId (byte.MaxValue, byte.MaxValue);
		public ItemId (byte category, byte variant) {
			this.category = category;
			this.variant = variant;
		}

		public ItemData data { get { return Manager.GetItem (this); } }

		// Overrides

		public override bool Equals (object obj) {
			if (!(obj is ItemId)) {
				return false;
			}

			ItemId other = (ItemId) obj;

			return this.category.Equals (other.category) && this.variant.Equals (other.variant);
		}

		public override int GetHashCode () {
			return this.category.GetHashCode () + 65565 * this.variant.GetHashCode ();
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

	public static class ItemExtensions {
		public static string ToPrint (this IEnumerable<ItemStack> stacks) {
			return "[" + string.Join (", ", stacks) + "]";
		}
	}
}