using System;
using Godot;

public class ItemSpawner : SpawnerComponent {
    [Export] public string itemName;

    override protected Body GetSpawnee () {
        return Item.Builder.MakeBody (itemName);
    }

}