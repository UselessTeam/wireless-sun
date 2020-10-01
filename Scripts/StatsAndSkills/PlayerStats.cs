using System;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;

namespace Stats {
	public class PlayerStats : Node {
		public Dictionary<string, Stat> statsList;

		public override void _Ready () {
			AddToGroup ("SaveNodes");
			GameRoot.playerStats = this;
			statsList = MakeStatList ();
		}

		public Stat GetStat (string name) {
			if (statsList == null) {
				GD.PrintErr ("Stats were not initialized yet");
				return null;
			}
			Stat returnElm;
			statsList.TryGetValue (name, out returnElm);
			return returnElm;
		}

		public static Dictionary<string, Stat> MakeStatList () {
			Dictionary<string, Stat> statList = new Dictionary<string, Stat> {
				["root"] = new Stat (
					"root", "",
					(stat) => { return stat.level; },
					(stat) => { return "No description yet ..."; } //TODO
				) { level = 1 },
				["body"] = new Stat (
					"body", "root",
					(stat) => { return stat.level; },
					(stat) => { return "Boos XP gain for all body stats"; } //TODO

				),
				["combat"] = new Stat ("combat", "root",
					(stat) => { return stat.level; },
					(stat) => { return "Boost XP gain for all combat stats"; } //TODO
				),
				["weapon"] = new Stat (
					"weapon", "root",
					(stat) => { return stat.level; },
					(stat) => { return "Boost XP gain for all weapons"; } //TODO
				),
				["health"] = new Stat (
					"health", "body",
					(stat) => { return stat.level; },
					(stat) => { return "+" + stat.Value + "HP"; } //TODO
				),
				["regen"] = new Stat (
					"regen", "body",
					(stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "Health regen +" + stat.Value + "%"; } //TODO
				),
				["speed"] = new Stat (
					"speed", "body",
					(stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "Walking speed +" + stat.Value + "%"; } //TODO
				),
				["damage"] = new Stat (
					"damage", "combat", (stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "Damage +" + stat.Value + "%"; } //TODO
				),
				["atk_speed"] = new Stat (
					"atk_speed", "combat",
					(stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "Attack speed +" + stat.Value + "%"; } //TODO
				),
				["crit"] = new Stat (
					"crit", "combat",
					(stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "[TODO] Critical chance +" + stat.Value + "%"; } //TODO
				),
				["axe"] = new Stat (
					"axe", "weapon",
					(stat) => { return stat.level * 5; },
					(stat) => { return "Charge attack damage +" + stat.Value; } //TODO
				),
				["bare_hand"] = new Stat (
					"bare_hand", "weapon",
					(stat) => { return stat.level; },
					(stat) => { return "No description yet ..."; } //TODO
				),
				["pickaxe"] = new Stat (
					"pickaxe", "weapon",
					(stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "Attack speed +" + stat.Value; } //TODO
				),
				["sword"] = new Stat (
					"sword", "weapon",
					(stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "Attack range +" + stat.Value + "%"; } //TODO
				),
				["shield"] = new Stat (
					"shield", "weapon",
					(stat) => { return 1 + stat.level / 100f; },
					(stat) => { return "Shield size +" + stat.Value + "%"; } //TODO
				),
			};
			return statList;
		}

		public Godot.Collections.Dictionary<string, object> MakeSave () {
			var saveObject = new Godot.Collections.Dictionary<string, object> () { { "Path", GetPath () } };
			foreach (var stat in statsList)
				saveObject[stat.Key] = stat.Value.Serialize ();
			return saveObject;

		}
		public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
			foreach (var stat in saveObject) {
				if (statsList.ContainsKey (stat.Key))
					statsList[stat.Key].Deserialize (stat.Value.ToString ());
			}
			// EmitSignal (nameof (UpdateTree));
		}

	}

}