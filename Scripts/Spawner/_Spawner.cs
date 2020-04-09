using System;
using Godot;

public abstract class _Spawner : Node2D {
	const int NON_SPAWNEE_CHILDREN = 1;

	[Export] public float SPAWN_DELAY = 2;
	[Export] public int RANDOM_TIME_SPREAD = 50;
	[Export] public float MAX_COUNT = 10;

	// public double next_spawn_delay;

	public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
	public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

	public float Radius { get { return ((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius; } }

	double timeUntilNext;

	int spawnID = 0;

	public override void _Ready () {
		// GD.Print (GetTree ().GetNetworkUniqueId ().ToString () + " " + IsNetworkMaster () + Network.IsConnectionStarted);
		if (!IsMaster)
			Rpc ("SendAllSpawnees", GetTree ().GetNetworkUniqueId ());
		if (IsMaster)
			AddToGroup ("SaveNodes");
		timeUntilNext = NextSpawnDelay ();
	}

	public double NextSpawnDelay () { return SPAWN_DELAY * (1 + General.rng.Next (-RANDOM_TIME_SPREAD, RANDOM_TIME_SPREAD) / 100.0); }

	public override void _Process (float delta) {
		if (IsMaster && IsActive ()) {
			timeUntilNext -= delta;
			if (timeUntilNext <= 0) {
				timeUntilNext = SPAWN_DELAY * (1 + General.rng.Next (-RANDOM_TIME_SPREAD, RANDOM_TIME_SPREAD) / 100.0);
				var position = GenerateSpawnPosition (Radius);
				if (Network.IsConnectionStarted)
					Rpc ("SpawnOne", spawnID.ToString (), position);
				else
					SpawnOne (spawnID.ToString (), position);
				spawnID++;
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
		spawnBody.Position = position;
		AddChild (spawnBody);
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
		GD.Print (saveObject);
		Position = new Vector2 (Convert.ToSingle (saveObject["PositionX"]), Convert.ToSingle (saveObject["PositionY"]));
		timeUntilNext = Convert.ToSingle (saveObject["TimeUntilNext"]);
		spawnID = Convert.ToInt32 (saveObject["SpawnID"]);
	}
}