using System;
using Godot;

public class PrefabSpawner : SpawnerComponent {
	[Export] public PackedScene spawnPrefab;

	override protected IPiece GetSpawnee () {
		return spawnPrefab.Instance ().GetNode<IPiece> ("./");
	}
}
