using System;
using Godot;

public class _Spawner : Node2D {
	const int NON_SPAWNEE_CHILDREN = 1;

	[Export] public float SPAWN_DELAY = 2;
	[Export] public float MAX_COUNT = 10;
	[Export] public PackedScene SpawnPrefab;

	public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
	public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

	public float Radius { get { return ((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius; } }

	float timeSinceLastSpawn;

	int spawnID = 0;

	public override void _Ready () {
		// GD.Print (GetTree ().GetNetworkUniqueId ().ToString () + " " + IsNetworkMaster () + Network.IsConnectionStarted);
		if (!IsMaster)
			Rpc ("SendAllSpawnees", GetTree ().GetNetworkUniqueId ());

	}

	public override void _Process (float delta) {
		if (IsMaster && IsActive ()) {
			timeSinceLastSpawn += delta;
			if (timeSinceLastSpawn > SPAWN_DELAY) {
				timeSinceLastSpawn = 0;
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

	[PuppetSync]
	void SpawnOne (string name, Vector2 position) {
		var spawnBody = SpawnPrefab.Instance ().GetNode<Body> ("./");
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
