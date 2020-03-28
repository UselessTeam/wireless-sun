using System;
using Godot;

public class PrefabSpawner : _Spawner {
	[Export] public PackedScene spawnPrefab;

	override protected Body GetSpawnee () {
		return spawnPrefab.Instance ().GetNode<Body> ("./");
	}
}