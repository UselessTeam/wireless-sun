using System;
using Godot;

public class ItemSpawner : SpawnerComponent {
	public static PackedScene template = (PackedScene) ResourceLoader.Load ("res://Nodes/Spawners/ItemSpawner.tscn");
	public static ItemSpawner Instance () {
		return (ItemSpawner) template.Instance ();
	}
	public static ItemSpawner Instance (string itemName, float radius = -1, float spawnDelay = 2, int randomTimeSpread = 50, int maxCount = 10, int maxTries = 1) {
		ItemSpawner instance = Instance ();
		instance.itemName = itemName;
		if (radius >= 0) {
			instance.Radius = radius;
		}
		instance.SpawnDelay = spawnDelay;
		instance.RandomTimeSpread = randomTimeSpread;
		instance.MaxCount = maxCount;
		instance.MaxTries = maxTries;
		return instance;
	}

	[Export] public string itemName;

	override protected IPiece GetSpawnee () {
		return Item.Builder.MakePiece (itemName);
	}

}
