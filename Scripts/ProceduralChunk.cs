using Godot;
using System;
using System.Collections.Generic;

public class ProceduralChunk : TileMap
{
	enum Biom : byte {
		Void = 0,
		Sea = 1,
		Sand = 2,
		Grass = 3,
		Stone = 4,
	}

	struct CellType {
		public Biom top_left;
		public Biom top_right;
		public Biom bottom_left;
		public Biom bottom_right;

		public bool NoVoid() {
			return (top_left != Biom.Void)
				&& (top_right != Biom.Void)
				&& (bottom_left != Biom.Void)
				&& (bottom_right != Biom.Void);
		}

		public CellType(Biom top_left, Biom top_right, Biom bottom_left, Biom bottom_right) {
			this.top_left = top_left;
			this.top_right = top_right;
			this.bottom_left = bottom_left;
			this.bottom_right = bottom_right;
		}

		public override String ToString() {
			return "[" + top_left + "," + top_right + "," + bottom_left + "," + bottom_right + "]";
		}
	}

	private const uint TOP_LEFT = 1;
	private const uint TOP_RIGHT = 4;
	private const uint BOTTOM_LEFT = 64;
	private const uint BOTTOM_RIGHT = 256;

	private Biom[] layers = { Biom.Sea, Biom.Sand, Biom.Grass, Biom.Stone };

	private Vector2 size;

	private Dictionary<CellType, List<Vector2>> tiles = new Dictionary<CellType, List<Vector2>> ();

	private void AddCell(CellType cell, Vector2 id) {
		if (!tiles.ContainsKey(cell)) {
			tiles.Add(cell, new List<Vector2> {id});
		} else {
			tiles[cell].Add(id);
		}
	}

	static Random rnd = new Random();

	private Vector2 GetCell(CellType cell) {
		if (!tiles.ContainsKey(cell)) {
			return Vector2.Zero;
		} else {
			return tiles[cell][rnd.Next(tiles[cell].Count)];
		}
	}

	private Biom[][] map = {
		new Biom[] { Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sea, Biom.Sand, Biom.Sand, Biom.Sand, Biom.Sand, Biom.Sand, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sand, Biom.Sand, Biom.Sea, Biom.Sand, Biom.Sand, Biom.Sand, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sand, Biom.Sea, Biom.Sea, Biom.Sand, Biom.Sand, Biom.Sand, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea },
		new Biom[] { Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea, Biom.Sea },
	};

	[Export]
	private OpenSimplexNoise noise; // = new OpenSimplexNoise();

	public override void _Ready()
	{
		size = TileSet.AutotileGetSize(0);
		for(int x = 0; x < size.x; x++) {
			for(int y = 0; y < size.y; y++) {
				Vector2 coord = new Vector2(x,y);
				CellType cell = new CellType();
				for(int l = 0; l < layers.Length; l++) {
					var bitmask = TileSet.AutotileGetBitmask(l, coord);
					if((bitmask & TOP_LEFT) != 0) {
						cell.top_left = layers[l];
					}
					if((bitmask & TOP_RIGHT) != 0) {
						cell.top_right = layers[l];
					}
					if((bitmask & BOTTOM_LEFT) != 0) {
						cell.bottom_left = layers[l];
					}
					if((bitmask & BOTTOM_RIGHT) != 0) {
						cell.bottom_right = layers[l];
					}
				}
				if (cell.NoVoid()) {
					AddCell(cell, coord);					
				}
			}
		}

		Generate(0,0);
	}

	const int CHUNK_SIZE = 64;
	public void Generate(int X, int Y) {
		for(int y = 0; y < CHUNK_SIZE; y++) {
			for(int x = 0; x < CHUNK_SIZE; x++) {
				CellType cell = MapCell(x + X * CHUNK_SIZE, y + Y * CHUNK_SIZE);
				var coord = GetCell(cell);
				SetCell(x, y, 0, autotileCoord: coord);
			}
		}
	}
	private CellType MapCell(int x, int y) {
		return new CellType(Map(x, y), Map(x+1, y), Map(x, y+1), Map(x+1, y+1));
	}

	private Biom Map(int x, int y) {
		float main_value = noise.GetNoise2d(x, y);
		float secondary_value = noise.GetNoise2d(1200 - x, y - 1200);
		if (main_value + 0.2*Math.Abs(secondary_value) < -0.15) {
			return Biom.Sea;
		}
		if (main_value + Math.Abs(secondary_value) < 0.25) {
			return Biom.Sand;
		}
		if (secondary_value < 0.05) {
			return Biom.Grass;
		} else {
			return Biom.Stone;
		}
	}
}
