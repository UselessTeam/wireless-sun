using System;
using Godot;

public class PrefabSpawner : SpawnerComponent {
	[Export] public PackedScene spawnPrefab;

	override protected Body GetSpawnee () {
		return spawnPrefab.Instance ().GetNode<Body> ("./");
	}
}