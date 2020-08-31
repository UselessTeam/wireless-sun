using System;
using Godot;

public class PrefabSpawner : SpawnerComponent {
	public static PackedScene template = (PackedScene) ResourceLoader.Load ("res://Nodes/Spawners/PrefabSpawner.tscn");
	public static PrefabSpawner Instance () {
		return (PrefabSpawner) template.Instance ();
	}
	public static PrefabSpawner Instance (PackedScene spawnPrefab, float radius = -1, float spawnDelay = 2, int randomTimeSpread = 50, int maxCount = 10, int maxTries = 1) {
		PrefabSpawner instance = Instance ();
		instance.spawnPrefab = spawnPrefab;
		if (radius >= 0) {
			instance.Radius = radius;
		}
		instance.SpawnDelay = spawnDelay;
		instance.RandomTimeSpread = randomTimeSpread;
		instance.MaxCount = maxCount;
		instance.MaxTries = maxTries;
		return instance;
	}

	[Export] public PackedScene spawnPrefab;

	override protected IPiece GetSpawnee () {
		return spawnPrefab.Instance ().GetNode<IPiece> ("./");
	}
}
