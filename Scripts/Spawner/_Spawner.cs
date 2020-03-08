using System;
using Godot;

public class _Spawner : Node2D {
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
			GD.Print ("Spawn everyone");
	}

	public override void _Process (float delta) {
		if (IsMaster && IsActive ()) {
			timeSinceLastSpawn += delta;
			if (timeSinceLastSpawn > SPAWN_DELAY) {
				timeSinceLastSpawn = 0;
				var position = GenerateSpawnPosition ();
				if (Network.IsConnectionStarted)
					Rpc ("SpawnOne", position, spawnID);
				else
					SpawnOne (position, spawnID);
				spawnID++;
			}
		}
	}

	// Weather the spawner is active
	bool IsActive () { return GetChildCount () < MAX_COUNT; }

	Vector2 GenerateSpawnPosition () {
		double theta = General.rng.NextDouble () * 2 * Math.PI;
		return new Vector2 ((float) Math.Cos (theta), (float) Math.Sin (theta)) * (float) General.rng.NextDouble () * Radius;

	}

	[PuppetSync]
	void SpawnOne (Vector2 position, int id) {
		var spawnBody = SpawnPrefab.Instance ().GetNode<Body> ("./");
		spawnBody.Name = id.ToString ();
		spawnBody.Position = position;
		AddChild (spawnBody);
	}

	void CheckChildren () {

	}

	[Puppet]
	void CheckChild (string Name) {

	}

}
