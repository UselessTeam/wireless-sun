using System;
using Godot;

public class ItemSpawner : _Spawner {
    [Export] public string itemName;

    override protected Body GetSpawnee () {
        return Item.Builder.MakeBody (itemName);
    }

}