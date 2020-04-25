using System;
using Godot;

public abstract class _Spawner : Node2D {
	int NON_SPAWNEE_CHILDREN = 1;

	[Export] public float SPAWN_DELAY = 2; // Time (in seconds) between two spawns
	[Export] public int RANDOM_TIME_SPREAD = 50; // Random spread of the time between two spawns
	[Export] public float MAX_COUNT = 10; // Max number of spawned objects exising at the same time
	[Export] public float MAX_TRIES = 1; // Number of times this node will try to spawn an object 

	// public double next_spawn_delay;

	public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
	public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

	public float Radius {
		get { return ((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius; }
		set {
			((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius = value;
		}
	}

	double timeUntilNext;

	int spawnID = 0;

	Area2D checkArea = null;

	public override void _Ready () {
		NON_SPAWNEE_CHILDREN = GetChildCount ();
		if (!IsMaster)
			Rpc ("SendAllSpawnees", GetTree ().GetNetworkUniqueId ());
		if (IsMaster)
			AddToGroup ("SaveNodes");
		timeUntilNext = NextSpawnDelay ();
		checkArea = GetNodeOrNull<Area2D> ("CheckArea");
	}

	public double NextSpawnDelay () { return SPAWN_DELAY * (1 + General.rng.Next (-RANDOM_TIME_SPREAD, RANDOM_TIME_SPREAD) / 100.0); }

	public override void _Process (float delta) {
		if (IsMaster && IsActive ()) {
			timeUntilNext -= delta;
			if (timeUntilNext <= 0) {
				timeUntilNext = SPAWN_DELAY * (1 + General.rng.Next (-RANDOM_TIME_SPREAD, RANDOM_TIME_SPREAD) / 100.0);
				Vector2 position;
				ushort numberTries = 0;
				do {
					position = GenerateSpawnPosition (Radius);
					checkArea.Position = position;
					// Dead debug code
					// I keep it just in case
					//
					if (GetWorld2d ().DirectSpaceState.IntersectPoint (checkArea.GlobalPosition, 1, null, 1024, true, true).Count == 0) {
						if (Network.IsConnectionStarted)
							Rpc ("SpawnOne", spawnID.ToString (), position);
						else
							SpawnOne (spawnID.ToString (), position);
						spawnID++;
					}
					if (numberTries > MAX_TRIES) {
						numberTries = 0;
						break;
					}
					numberTries++;
				} while (numberTries < MAX_TRIES);
			}
		}
	}

	int GetNumberSpawnees () {
		return GetChildCount () - NON_SPAWNEE_CHILDREN;
	}

	// Weather the spawner is active
	bool IsActive () { return GetNumberSpawnees () < MAX_COUNT; }

	public static Vector2 GenerateSpawnPosition (float Radius) {
		double theta = General.rng.NextDouble () * 2 * Math.PI;
		return new Vector2 ((float) Math.Cos (theta), (float) Math.Sin (theta)) * (float) General.rng.NextDouble () * Radius;

	}

	abstract protected Body GetSpawnee ();

	[PuppetSync]
	void SpawnOne (string name, Vector2 position) {
		var spawnBody = GetSpawnee ();
		spawnBody.Name = name;
		AddChild (spawnBody);
		spawnBody.Position = position;
	}

	[Master]
	void SendAllSpawnees (int id) {
		for (int i = 0; i < GetNumberSpawnees (); i++) {
			var spawnee = (Body) GetChild (i + NON_SPAWNEE_CHILDREN);
			var name = spawnee.Name;
			GD.Print ("Spawning ", name);
			var position = spawnee.Position;
			RpcId (id, "SpawnOne", name, new Vector2 (0, 0));
		}
	}

	public Godot.Collections.Dictionary<string, object> MakeSave () {
		var saveObject = new Godot.Collections.Dictionary<string, object> () { { "Path", GetPath () }, {
					"TimeUntilNext",
					timeUntilNext
				}, { "SpawnID", spawnID }, {
					"PositionX",
					Position.x
				}, { "PositionY", Position.y }
			};
		return saveObject;
	}
	public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		Position = new Vector2 (Convert.ToSingle (saveObject["PositionX"]), Convert.ToSingle (saveObject["PositionY"]));
		timeUntilNext = Convert.ToSingle (saveObject["TimeUntilNext"]);
		spawnID = Convert.ToInt32 (saveObject["SpawnID"]);
	}
}