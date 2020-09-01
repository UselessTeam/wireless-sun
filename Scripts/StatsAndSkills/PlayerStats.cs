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
			Dictionary<string, Stat> rootStat = new Dictionary<string, Stat> {
				["root"] = new Stat {
					name = "root", fatherStat = "", level = 1,
						value = (stat) => { return stat.level; },
						description = (stat) => { return "No description yet ..."; }, //TODO
				},
				["body"] = new Stat {
					name = "body", fatherStat = "root",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "No description yet ..."; }, //TODO

				},
				["combat"] = new Stat {
					name = "combat", fatherStat = "root",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "No description yet ..."; }, //TODO
				},
				["weapon"] = new Stat {
					name = "weapon", fatherStat = "root",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "No description yet ..."; }, //TODO
				},
				["health"] = new Stat {
					name = "health", fatherStat = "body",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "+" + stat.value + "HP"; }, //TODO
				},
				["regen"] = new Stat {
					name = "regen", fatherStat = "body",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "Health regen +" + stat.value + "%"; }, //TODO
				},
				["speed"] = new Stat {
					name = "speed", fatherStat = "body",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "Walking speed +" + stat.value + "%"; }, //TODO
				},
				["attack"] = new Stat {
					name = "attack", fatherStat = "combat",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "Damage +" + stat.value + "%"; }, //TODO
				},
				["block"] = new Stat {
					name = "block", fatherStat = "combat",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "No description yet ..."; }, //TODO
				},
				["crit"] = new Stat {
					name = "crit", fatherStat = "combat",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "Critical chance +" + stat.value + "%"; }, //TODO
				},
				["axe"] = new Stat {
					name = "axe", fatherStat = "weapon",
						value = (stat) => { return stat.level * 5; },
						description = (stat) => { return "Charge attack damage +" + stat.value; }, //TODO
				},
				["bare_hand"] = new Stat {
					name = "bare_hand", fatherStat = "weapon",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "No description yet ..." ;}, //TODO
				},
				["pickaxe"] = new Stat {
					name = "pickaxe", fatherStat = "weapon",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "Attack speed +" + stat.value; }, //TODO
				},
				["sword"] = new Stat {
					name = "sword", fatherStat = "weapon",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "Attack range +" + stat.value + "%"; }, //TODO
				},
				["shield"] = new Stat {
					name = "shield", fatherStat = "shield",
						value = (stat) => { return stat.level; },
						description = (stat) => { return "Shield size +" + stat.value + "%"; }, //TODO
				},
			};
			return rootStat;
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
