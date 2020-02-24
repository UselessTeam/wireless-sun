using Godot;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Item {
	public static class Manager {
		private static ItemCategory[] categories;

		private static Dictionary<string, ItemId> itemNames = new Dictionary<string, ItemId> ();
		public static void Load() {
			Godot.File file = new Godot.File();
			file.Open("res://Data/items.json", Godot.File.ModeFlags.Read);
			categories = JsonConvert.DeserializeObject<ItemCategory[]>(file.GetAsText());
			byte categoryId = 0;
			foreach(ItemCategory category in categories) {
				category.id = categoryId;
				byte variantId = 0;
				foreach(ItemData item in category.variants) {
					item.id = new ItemId(categoryId, variantId);
					itemNames[item.name] = item.id;
					variantId += 1;
				}
				categoryId += 1;
			}
		}

		public static ItemCategory GetCategory(byte categoryId) {
			if(categoryId == byte.MaxValue) {
				return null;
			}
			return categories[categoryId];
		}

		public static ItemData GetItem(ItemId id) {
			if(id == ItemId.NULL) {
				return null;
			}
			return GetCategory(id.category).variants[id.variant];
		}

		public static ItemData GetItem(string name) {
			ItemId id;
			if (itemNames.TryGetValue (name, out id))
				return GetItem(id);
			GD.PrintErr ("Error: Trying to get a non existing item : " + name);
			return new ItemData (name);
		}
	}

	public class ItemCategory {
		public byte id = 0;
		
		[JsonProperty("category")]
		public string name;
		public ItemData[] variants;
	}
	
	public struct ItemId {
		public byte category;
		public byte variant;

		public static readonly ItemId NULL = new ItemId(byte.MaxValue, byte.MaxValue);
		public ItemId(byte category, byte variant) {
			this.category = category;
			this.variant = variant;
		}

		public override bool Equals(object obj) {
			if (!(obj is ItemId)) {
				return false;                
			}

			ItemId other = (ItemId) obj;

			return this.category.Equals(other.category) && this.variant.Equals(other.variant);
		}

		public override int GetHashCode()
		{
			return this.category.GetHashCode() + 65565*this.variant.GetHashCode();
		}

		public static bool operator ==(ItemId a, ItemId b) 
		{
			return a.Equals(b);
		}

		public static bool operator !=(ItemId a, ItemId b) 
		{
		return !a.Equals(b);
		}
	}

	public class ItemData {
		public ItemId id;
		public ItemCategory category { get { return Manager.GetCategory(id.category); } }

		public int stack;
		public string name;

		public int frame;

		public ItemData() {
			this.id = ItemId.NULL;
			this.name = null;
		}
		public ItemData (ItemId id, string name = "NullName") {
			this.id = id;
			this.name = name;
		}

		public ItemData (string name = "NullName") {
			this.id = ItemId.NULL;
			this.name = name;
		}

		public override string ToString () {
			return name + " of type " + category + " (id=" + id.ToString () + ")";
		}
	}

	public struct ItemStack {
		ItemId itemId;
		int size;
	}
}
