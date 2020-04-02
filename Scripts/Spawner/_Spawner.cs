using System;
using Godot;

public abstract class _Spawner : Node2D {
	const int NON_SPAWNEE_CHILDREN = 1;

	[Export] public float SPAWN_DELAY = 2;
	[Export] public int RANDOM_SPREAD = 50;
	[Export] public float MAX_COUNT = 10;

	public double next_spawn_delay;

	public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
	public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

	public float Radius { get { return ((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius; } }

	float timeSinceLastSpawn;

	int spawnID = 0;

	public override void _Ready () {
		// GD.Print (GetTree ().GetNetworkUniqueId ().ToString () + " " + IsNetworkMaster () + Network.IsConnectionStarted);
		if (!IsMaster)
			Rpc ("SendAllSpawnees", GetTree ().GetNetworkUniqueId ());
		next_spawn_delay = SPAWN_DELAY * (1 + General.rng.Next (-RANDOM_SPREAD, RANDOM_SPREAD) / 100.0);

	}

	public override void _Process (float delta) {
		if (IsMaster && IsActive ()) {
			timeSinceLastSpawn += delta;
			if (timeSinceLastSpawn > next_spawn_delay) {
				timeSinceLastSpawn = 0;
				next_spawn_delay = SPAWN_DELAY * (1 + General.rng.Next (-RANDOM_SPREAD, RANDOM_SPREAD) / 100.0);
				var position = GenerateSpawnPosition ();
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

	Vector2 GenerateSpawnPosition () {
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
}