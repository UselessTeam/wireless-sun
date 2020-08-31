using System;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;

namespace Stats {

	public class Stat : Node {
		public string name;
		public Array<string> subStats;

		public float xpForLevel1 = 10;
		public float xpScalling = 1.3f; //XP required is multiplied every level

		public int level = 0;
		public float currentXp = 0;

		public float xpForNextLevel { get { return xpForLevel1 * Godot.Mathf.Pow (xpScalling, level); } }

		public Func<Stat, float> value = (stat) => {
			return (float) stat.level;
		};

		[Signal] public delegate void LevelUp (string statName);

		public void GainXp (float xp) {
			currentXp += xp;
			while (currentXp >= xpForNextLevel) {
				currentXp -= xpForNextLevel;
				level++;
				EmitSignal (nameof (LevelUp), name);
			}
			//EmitSignal TreeUpdate
		}

		public string Serialize () {
			var saveObject = new Godot.Collections.Dictionary<string, object> () {};
			saveObject[nameof (name)] = name;
			saveObject[nameof (level)] = level;
			saveObject[nameof (currentXp)] = currentXp;
			return JSON.Print (saveObject);
		}
		public void Deserialize (string serializedData) {
			var data = JsonConvert.DeserializeObject<Godot.Collections.Dictionary<string, object>> (serializedData);
			name = (string) data[nameof (name)];
			level = Convert.ToInt32 (data[nameof (level)]);
			currentXp = Convert.ToSingle (data[nameof (currentXp)]);
		}
	}

}
