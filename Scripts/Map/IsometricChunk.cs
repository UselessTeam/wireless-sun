using System;
using Godot;
using MetaTile;

public class IsometricChunk : Node2D {
	public static PackedScene template = (PackedScene)ResourceLoader.Load("res://Nodes/Map/IsometricChunk.tscn");
	public static IsometricChunk Instance() {
		return (IsometricChunk) template.Instance();
	}

	public const int SIZE = 32;
	public const int CHUNK_HALF_WIDTH = Tile.HALF_WIDTH * SIZE;
	public const int CHUNK_HALF_HEIGHT = Tile.HALF_HEIGHT * SIZE;


	private IsometricMap map;
	public int U { get; private set; }
	public int V { get; private set; }

	private Tile[] tiles = new Tile[SIZE*SIZE];

	public void Setup (IsometricMap map, int u, int v) {
		this.map = map;
		this.U = u;
		this.V = v;
	}

	private bool modified = true;

	public override void _Draw () {
		if (modified) {
			modified = false;
			if(map == null) {
				GD.PrintErr("Chunk has not been setup");
			}
			this.Position = new Vector2 ((U-V)*CHUNK_HALF_WIDTH, (U+V)*CHUNK_HALF_HEIGHT);
			this.ZIndex = (U + V) * SIZE;
			for (int v = 0; v < SIZE; v++) {
				for (int u = 0; u < SIZE; u++) {
					Tile tile = Tile.Instance();
					if(map != null) {
						var (t, w) = map.GetTileType(u + U * SIZE, v + V * SIZE);
						tile.SetCoord(u, v, w);
						tile.SetType(t);
					} else {
						tile.SetCoord(u, v, 0);
					}
					AddChild(tile);
				}
			}
		}
	}

	private Tile GetTile(int u, int v) {
		return tiles[v*SIZE + u];
	}

	private void SetTile(int u, int v, Tile tile) {
		tiles[v*SIZE + u] = tile;
	}
}
