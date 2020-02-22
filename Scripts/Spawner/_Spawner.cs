using System;
using Godot;

public class _Spawner : Node2D
{
    [Export] public float SPAWN_DELAY = 2;
    [Export] public PackedScene SpawnPrefab;

    public float Radius { get { return ((CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape).Radius; } }

    float timeSinceLastSpawn;

    public override void _Process(float delta)
    {
        timeSinceLastSpawn += delta;
        if (timeSinceLastSpawn > SPAWN_DELAY)
        {
            timeSinceLastSpawn = 0;
            SpawnOne();
        }
    }
    void SpawnOne()
    {

        var spawnBody = SpawnPrefab.Instance().GetNode<Body>("./");
        GD.Print(General.rng.Next());
        double theta = General.rng.NextDouble() * 2 * Math.PI;
        spawnBody.Position = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * (float)General.rng.NextDouble() * Radius;
        AddChild(spawnBody);
    }

}