using System;
using System.Collections.Generic;
using System.Threading;
using Godot;
using MetaTile;

public class IsometricMap : Node2D {

	[Export]
	private OpenSimplexNoise noise; // = new OpenSimplexNoise();

	public override void _Ready () {
		generateAsyncStart = new ParameterizedThreadStart (GenerateAsyncObject);
	}

	private int? cachedX = null;
	private int? cachedY = null;

	private const int DISTANCE = 1;
	private const int BUFFER = 1;

	private int Distance (int x1, int y1, int x2, int y2) {
		return Math.Abs (x1 - x2) + Math.Abs (y1 - y2);
	}
	public override void _Process (float delta) {
		// URGENT TODO: Old code from orthogonal coordinates
		Vector2 playerPosition = new Vector2 (21, 4); // TODO: Get the player's actual position, so chunks can be generated around him when necessary
		int X = (int) (playerPosition.x / Chunk.PIXEL_SIZE);
		int Y = (int) (playerPosition.y / Chunk.PIXEL_SIZE);
		if (X != cachedX || Y != cachedY) {
			foreach ((int, int) key in chunks.Keys) {
				if (Distance (X, Y, key.Item1, key.Item2) > DISTANCE + BUFFER) {
					chunks[key].QueueFree ();
					chunks.Remove (key);
				}
			}
			for (int x = X - DISTANCE; x <= X + DISTANCE; x++) {
				for (int y = Y - DISTANCE; y <= Y + DISTANCE; y++) {
					if (!chunks.ContainsKey ((x, y))) {
						chunks[(x, y)] = GenerateAsync (x, y); // TODO: Put unloaded chunks somewhere else so they don't accidently get called
					}
				}
			}
			cachedX = X;
			cachedY = Y;
		}
	}

	private Dictionary < (int, int), IsometricChunk > chunks = new Dictionary < (int, int), IsometricChunk > ();

	private PackedScene baseChunk = (PackedScene) ResourceLoader.Load ("res://Nodes/Map/Chunk.tscn");

	private void GenerateAsyncObject (object chunk) {
		Generate ((IsometricChunk) chunk);
	}

	private ParameterizedThreadStart generateAsyncStart; // = new ParameterizedThreadStart(GenerateAsyncObject);

	public IsometricChunk GenerateAsync (int U, int V) {
		System.Threading.Thread generateAsyncThread = new System.Threading.Thread (generateAsyncStart);
		IsometricChunk chunk = IsometricChunk.Instance ();
		chunk.Setup (this, U, V);
		generateAsyncThread.Start (chunk);
		return chunk;
	}

	public IsometricChunk Generate (IsometricChunk chunk) {
		CallDeferred ("add_child", chunk);
		return chunk;
	}

	public (int, int) GetTileType (int u, int v) {
		float base_height = noise.GetNoise2d (u, v) + 0.3f;
		if (base_height < 0f) {
			return (0, 0);
		}
		float main_value = noise.GetNoise2d (1200 - u, v - 1200);
		// float secondary_value = noise.GetNoise2d (1200 - u, v - 1200);
		if (main_value < 0) {
			return (2, (int)(4 * base_height + 0.7));;
		}
		if (base_height + main_value < 0.4f) {
			return (1, (int)(4 * base_height + 0.5));
		} else {
			return (3, (int)(4 * base_height + 0.9));
		}
	}
}
