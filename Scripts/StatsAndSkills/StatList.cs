using System;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;

namespace Stats {

	public class Stat : Node {
		public string name;
		public string fatherStat;

		public float xpForLevel1 = 10; //XP required to reach level 1
		public float xpScalling = 1.3f; //XP required is multiplied every level

		public int level = 0; // current level
		public float currentXp = 0; // current accumulated XP (is reset to 0 every level)

		public int fatherLevelRequired = 1; // Is stat/skill is unlocked when the father reaches this level

		public int xpForNextLevel { get { return Convert.ToInt32 (xpForLevel1 * Godot.Mathf.Pow (xpScalling, level)); } } //total Xp needed to rech next level

		public float Value { get { return value (this); } }
		public string Description { get { return description (this); } }

		private Func<Stat, float> value = (stat) => { //The useful value of this skill
			return (float) stat.level;
		};
		private Func<Stat, string> description = (stat) => { //Desciption to be displayed on the skill tree
			return "No description yet ...";
		};

		[Signal] public delegate void LevelUp (string statName);

		public Stat (string name, string fatherStat, Func<Stat, float> value, Func<Stat, string> description) {
			this.name = name;
			this.fatherStat = fatherStat;
			this.value = value;
			this.description = description;
		}

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