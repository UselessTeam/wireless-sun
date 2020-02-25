using System;
using Godot;

public class _Spawner : Node2D {
    [Export] public float SPAWN_DELAY = 2;
    [Export] public PackedScene SpawnPrefab;

    public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
    public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

    public float Radius { get { return ((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius; } }

    float timeSinceLastSpawn;

    public override void _Process (float delta) {
        if (IsMaster) {
            timeSinceLastSpawn += delta;
            if (timeSinceLastSpawn > SPAWN_DELAY) {
                timeSinceLastSpawn = 0;
                var position = GenerateSpawnPosition ();
                if (Network.IsConnectionStarted)
                    Rpc ("SpawnOne", position, GetTree ().GetNetworkUniqueId ());
                else
                    SpawnOne (position, GetTree ().GetNetworkUniqueId ());
            }
        }
    }

    Vector2 GenerateSpawnPosition () {
        double theta = General.rng.NextDouble () * 2 * Math.PI;
        return new Vector2 ((float) Math.Cos (theta), (float) Math.Sin (theta)) * (float) General.rng.NextDouble () * Radius;

    }

    [PuppetSync]
    void SpawnOne (Vector2 position, int id = 0) {
        GD.Print ("On " + GetTree ().GetNetworkUniqueId ().ToString () + " Spawning one from: " + id.ToString ());
        var spawnBody = SpawnPrefab.Instance ().GetNode<Body> ("./");
        spawnBody.Position = position;
        AddChild (spawnBody);
    }

}