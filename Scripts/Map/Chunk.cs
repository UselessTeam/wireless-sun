using System;
using System.Collections.Generic;
using Godot;

public class Chunk : Node2D {
	public static PackedScene template = (PackedScene) ResourceLoader.Load ("res://Nodes/Map/Chunk.tscn");
	public static Chunk Instance () {
		return (Chunk) template.Instance ();
	}

	private bool generated = false;
	public const int SIZE = 32;
	public const int CHUNK_HALF_WIDTH = Tile.HALF_WIDTH * SIZE;
	public const int CHUNK_HALF_HEIGHT = Tile.HALF_HEIGHT * SIZE;

	private Map map;
	public int U { get; private set; }
	public int V { get; private set; }

	private Tile[] tiles = new Tile[SIZE * SIZE];

	public void Setup (Map map, int u, int v) {
		this.map = map;
		this.U = u;
		this.V = v;
	}

	public void Generate () {
		if (!generated) {
			MakeSpawners ();
			MakeTiles ();
		}
		generated = true;
	}

	public override void _Ready () {
		Generate ();
	}

	private void MakeTiles () {
		this.Position = new Vector2 ((U - V) * CHUNK_HALF_WIDTH, (U + V) * CHUNK_HALF_HEIGHT);
		this.ZIndex = (U + V) * SIZE;
		for (int v = 0; v < SIZE; v++) {
			for (int u = 0; u < SIZE; u++) {
				var (t, w) = (TileType.NONE, 0);
				if (map != null) {
					(t, w) = map.GetTileType (u + U * SIZE, v + V * SIZE);
				}
				AddChild (Tile.Instance (u, v, w, t));
			}
		}
	}

	private Tile GetTile (int u, int v) {
		return tiles[v * SIZE + u];
	}

	private void SetTile (int u, int v, Tile tile) {
		tiles[v * SIZE + u] = tile;
	}

	// Procedural Spawner Generation

	private static Dictionary < string, (float, float, float, float, float) > FORMULAS = new Dictionary < string, (float, float, float, float, float) > {
		["berry"] = (0, 0.1f, -0.1f, 0.3f, 0.7f),
		// ["stick"] = (-0.1f, 0f, 0.2f, -0.2f, 0.7f),
		["mushroom"] = (0.1f, -0.1f, -0.2f, 0.8f, 0.1f),
		["ore"] = (0f, 0.2f, 0.7f, 0f, -0.5f),
		["stone"] = (0.025f, 0.3f, 0.6f, 0.1f, -0.4f),

		["tree"] = (0.1f, -0.2f, 0.3f, -0.1f, 0.6f),
		["spiky"] = (0.1f, 0.1f, 0.5f, 0.3f, -0.4f),
	};
	private void MakeSpawners () {
		List<Vector2> positions = map.GetSpawnerPositions (U, V);
		Dictionary < string, (int, float) > candidates = new Dictionary < string, (int, float) > ();

		// Generates candidates of what can be spawn, and for each gives the most suttable position and a score
		int i = 0;
		foreach (var position in positions) {
			int u = (int) position.x;
			int v = (int) position.y;
			Map.Flavor flavor = map.GetCoordFlavor (u + U * SIZE, v + V * SIZE);
			foreach (var pair in FORMULAS) {
				var (bas, alt, hea, mud, veg) = pair.Value;
				float value = bas + alt * flavor.altitude + hea * flavor.heavyness + mud * flavor.muddyness + veg * flavor.vegetation;
				if (value > 0 && flavor.altitude > 0) {
					float oldValue = 0;
					if (candidates.ContainsKey (pair.Key)) {
						oldValue = candidates[pair.Key].Item2;
					}
					if (value > oldValue) {
						candidates[pair.Key] = (i, value);
					}
				}
			}
			i++;
		}

		// Takes the best candidates
		bool[] used = new bool[positions.Count];
		for (int l = 0; l < positions.Count; l++) {
			float currentMax = 0;
			string currentSpawned = null;
			foreach (var candidate in candidates) {
				var (k, value) = candidate.Value;
				if (used[k] || value <= currentMax) {
					continue;
				}
				currentMax = value;
				currentSpawned = candidate.Key;
			}
			if (currentSpawned is null) {
				break;
			}
			int spawnedK = candidates[currentSpawned].Item1;
			used[spawnedK] = true;
			Spawn (currentSpawned, positions[spawnedK], currentMax);
		}
	}

	private static Dictionary<string, PackedScene> PACKED_SPAWNERS = new Dictionary<string, PackedScene> {
		["tree"] = (PackedScene) ResourceLoader.Load ("res://Nodes/Bodies/Tree.tscn"),
		["spiky"] = (PackedScene) ResourceLoader.Load ("res://Nodes/Bodies/SpikyBody.tscn"),
	};

	private void Spawn (string what, Vector2 position, float score) {
		SpawnerComponent spawner;
		PackedScene packedScene;
		float radius = 120 + 100 * score;
		if (PACKED_SPAWNERS.TryGetValue (what, out packedScene)) {
			spawner = PrefabSpawner.Instance (packedScene, radius : radius);
		} else {
			spawner = ItemSpawner.Instance (what, radius : radius);
		}
		spawner.Name = "Spawner_" + what;
		AddChild (spawner);
		spawner.Position = Tile.TransposeCoord (position.x, position.y);
	}
}
