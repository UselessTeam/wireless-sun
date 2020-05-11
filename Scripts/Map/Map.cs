using System;
using System.Collections.Generic;
using System.Threading;
using Godot;

public class Map : Node2D {

    public static Map Global;

    [Export]
    private OpenSimplexNoise noise; // = new OpenSimplexNoise();

    public override void _Ready () {
        Global = this;
        generateAsyncStart = new ParameterizedThreadStart (GenerateAsyncObject);
        _Process (0);
    }

    private Dictionary < (int, int), Chunk > chunks = new Dictionary < (int, int), Chunk > ();

    private PackedScene baseChunk = (PackedScene) ResourceLoader.Load ("res://Nodes/Map/Chunk.tscn");

    // # Automatically generate Map around player

    private int? cachedU = null;
    private int? cachedV = null;

    private const int DISTANCE = 1;
    private const int BUFFER = 1;

    private int Distance (int u1, int v1, int u2, int v2) {
        return Math.Abs (u1 - u2) + Math.Abs (v1 - v2);
    }
    public override void _Process (float delta) {
        Vector2 playerPosition = new Vector2 (0, 0); // TODO: Get the player's actual position, so chunks can be generated around him when necessary
        var coord = Tile.TransposePosition (playerPosition);
        int U = Mathf.FloorToInt (coord.x / Chunk.SIZE);
        int V = Mathf.FloorToInt (coord.y / Chunk.SIZE);
        if (U != cachedU || V != cachedV) {
            foreach ((int, int) key in chunks.Keys) {
                if (Distance (U, V, key.Item1, key.Item2) > DISTANCE + BUFFER) {
                    chunks[key].QueueFree ();
                    chunks.Remove (key);
                }
            }
            for (int u = U - DISTANCE; u <= U + DISTANCE; u++) {
                for (int v = V - DISTANCE; v <= V + DISTANCE; v++) {
                    if (!chunks.ContainsKey ((u, v))) {
                        GD.Print ("Generating Chunk " + u + " " + v);
                        chunks[(u, v)] = Generate (u, v); // TODO: Put unloaded chunks somewhere else so they don't accidently get called
                    }
                }
            }
            cachedU = U;
            cachedV = V;
        }
    }

    private void GenerateAsyncObject (object chunk) {
        GenerateDeffered ((Chunk) chunk);
    }

    private ParameterizedThreadStart generateAsyncStart; // = new ParameterizedThreadStart(GenerateAsyncObject);

    public Chunk GenerateAsync (int U, int V) {
        System.Threading.Thread generateAsyncThread = new System.Threading.Thread (generateAsyncStart);
        Chunk chunk = Chunk.Instance ();
        chunk.Setup (this, U, V);
        chunk.Name = "Chunk_" + U + "_" + V;
        generateAsyncThread.Start (chunk);
        return chunk;
    }
    public Chunk GenerateDeffered (Chunk chunk) {
        chunk.Generate ();
        CallDeferred ("add_child", chunk);
        return chunk;
    }

    public Chunk Generate (int U, int V) {
        Chunk chunk = Chunk.Instance ();
        chunk.Setup (this, U, V);
        chunk.Name = "Chunk_" + U + "_" + V;
        chunk.Generate ();
        AddChild (chunk);
        return chunk;
    }

    // # Usefull public functions

    public struct Flavor {
        public float altitude;
        public float heavyness;
        public float muddyness;
        public float vegetation;
    }

    public Flavor GetCoordFlavor (float u, float v) {
        Flavor flavor = new Flavor ();
        flavor.altitude = noise.GetNoise2d (u, v) + 0.7f - 0.001f * (u * u + v * v);
        flavor.heavyness = noise.GetNoise2d (0.8f * u + 780f, 0.8f * v + 1000f) + 0.3f * flavor.altitude;
        flavor.muddyness = noise.GetNoise2d (u + 600f, v - 2000f) + 0.3f * flavor.heavyness;
        flavor.vegetation = 0.5f * flavor.muddyness + noise.GetNoise2d (1200f - u, v - 1200f);
        return flavor;
    }

    public Flavor GetPositionFlavor (Vector2 position) {
        Vector2 coords = Tile.TransposePosition (position);
        return GetCoordFlavor (coords.x, coords.y);
    }

    public (TileType, int) GetTileType (int u, int v) {
        Flavor flavor = GetCoordFlavor (u, v);
        if (flavor.altitude <= 0f) {
            return (TileType.SEA, 0);
        }
        if (flavor.altitude + flavor.heavyness < 0.2f) {
            return (TileType.SAND, 0);
        }
        if (flavor.muddyness < 0f) {
            if (flavor.heavyness < 0f) {
                return (TileType.SAND, 0);
            } else {
                return (TileType.STONE, 0);
            }
        } else {
            if (flavor.vegetation > -0.1f) {
                return (TileType.GRASS, 0);
            } else {
                return (TileType.DIRT, 0);
            }

        }
    }

    public List<Vector2> GetSpawnerPositions (int u, int v) {
        List<Vector2> positions = new List<Vector2> ();
        int spawnersCount = 8;
        for (int c = 0; c < spawnersCount; c++) {
            int sU = (int) (1024 * (2 + noise.GetNoise2d (128 * u - 7, 1024 * v + 111 * c)) % Chunk.SIZE);
            int sV = (int) (1024 * (2 + noise.GetNoise2d (128 * u + 7, 1024 * v - 111 * c)) % Chunk.SIZE);
            positions.Add (new Vector2 (sU, sV));
        }
        return positions;
    }
}