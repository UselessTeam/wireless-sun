using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public class Map : Node2D
{

	[Export]
	private OpenSimplexNoise noise; // = new OpenSimplexNoise();

	private SmartTiles tiles;

	public override void _Ready()
	{
		tiles = (SmartTiles)GetNode("/root/SmartTiles");
		generateAsyncStart = new ParameterizedThreadStart(GenerateAsyncObject);
	}

	private int? cachedX = null;
	private int? cachedY = null;

	private const int DISTANCE = 1;
	private const int BUFFER = 1;

	private int Distance(int x1, int y1, int x2, int y2) {
		return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
	}
	public override void _Process(float delta) {
		Vector2 playerPosition = new Vector2(21,4); // TODO: Get the player's actual position, so chunks can be generated around him when necessary
		int X = (int)(playerPosition.x / Chunk.PIXEL_SIZE);
		int Y = (int)(playerPosition.y / Chunk.PIXEL_SIZE);
		if (X != cachedX || Y != cachedY) {
			foreach((int,int) key in chunks.Keys) {
				if(Distance(X, Y, key.Item1, key.Item2) > DISTANCE + BUFFER) {
					chunks[key].QueueFree();
					chunks.Remove(key);
				}
			}
			for(int x = X - DISTANCE; x <= X + DISTANCE; x++) {
				for(int y = Y - DISTANCE; y <= Y + DISTANCE; y++) {
					if(!chunks.ContainsKey((x,y))) {
						chunks[(x,y)] = GenerateAsync(x,y); // TODO: Put unloaded chunks somewhere else so they don't accidently get called
					}
				}
			}
			cachedX = X;
			cachedY = Y;
		}
	}

	private Dictionary<(int, int), Chunk> chunks = new Dictionary<(int, int), Chunk>();

	private PackedScene baseChunk = ResourceLoader.Load("res://Nodes/Map/Chunk.tscn") as PackedScene;

	private void GenerateAsyncObject(object chunk) {
		Generate((Chunk) chunk);
	}
	private ParameterizedThreadStart generateAsyncStart;

	public Chunk GenerateAsync(int X, int Y) {
		System.Threading.Thread generateAsyncThread = new System.Threading.Thread(generateAsyncStart);
		Chunk chunk = (Chunk)baseChunk.Instance();
		chunk.Setup(tiles, X, Y);
		generateAsyncThread.Start(chunk);
		return chunk;
	}

	public Chunk Generate(Chunk chunk) {
		for(int x = 0; x < Chunk.SIZE + 1; x++) {
			for(int y = 0; y < Chunk.SIZE + 1; y++) {
				chunk.SetBiom(x, y, GetBiom(x + chunk.x * Chunk.SIZE, y + chunk.y * Chunk.SIZE));
			}
		}
		CallDeferred("add_child", chunk);
		return chunk;
	}

	public void Generate(int X, int Y) {
		Chunk chunk = (Chunk)baseChunk.Instance();
		chunk.Setup(tiles, X, Y);
		Generate(chunk);
	}

	public Biom GetBiomAtPosition(float x, float y) {
		return GetBiom((int)(x / Chunk.RESOLUTION), (int)(y / Chunk.RESOLUTION));
	}
	public Biom GetBiom(int x, int y) {
		float main_value = noise.GetNoise2d(x, y);
		float secondary_value = noise.GetNoise2d(1200 - x, y - 1200);
		if (main_value + 0.5*Math.Abs(secondary_value) < -0.15) {
			return Biom.Sea;
		}
		if (main_value + Math.Abs(secondary_value) < 0.15) {
			return Biom.Sand;
		}
		if (secondary_value < 0.05) {
			return Biom.Grass;
		} else {
			return Biom.Stone;
		}
	}
}
