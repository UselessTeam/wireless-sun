using System;
using Godot;

public class ItemSpawner : SpawnerComponent {
    [Export] public string itemName;

    override protected IPiece GetSpawnee () {
        return Item.Builder.MakePiece (itemName);
    }

}