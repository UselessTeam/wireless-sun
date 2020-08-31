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
				name = "root",
				subStats = new Array<string> {
				"body",
				"combat",
				"weapon"
				}
				},
				["body"] = new Stat {
				name = "body",
				subStats = new Array<string> {
				"health",
				"regen",
				"speed"
				}
				},
				["combat"] = new Stat {
				name = "combat",
				subStats = new Array<string> {
				"attack",
				"block",
				"crit"
				}
				},
				["weapon"] = new Stat {
				name = "weapon",
				subStats = new Array<string> {
				"bare_hands",
				"axe",
				"pickaxe",
				"sword"
				}
				},
				["health"] = new Stat { name = "health" },
				["regen"] = new Stat { name = "regen" },
				["speed"] = new Stat { name = "speed" },
				["attack"] = new Stat { name = "attack" },
				["block"] = new Stat { name = "block" },
				["crit"] = new Stat { name = "crit" },
				["axe"] = new Stat { name = "axe" },
				["pickaxe"] = new Stat { name = "pickaxe" },
				["sword"] = new Stat { name = "sword" },
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
