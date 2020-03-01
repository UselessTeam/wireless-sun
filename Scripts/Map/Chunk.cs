using Godot;
using System;

public class Chunk : TileMap
{
	private SmartTiles tiles;
	public int x { get; private set; }
	public int y { get; private set; }

	public void Setup(SmartTiles tiles, int x, int y) {
		this.tiles = tiles;
		this.x = x;
		this.y = y;
	}

	private bool modified = true;

	public override void _Draw() {
		if(modified) {
			modified = false;
			this.Position = new Vector2(Chunk.RESOLUTION * x * Chunk.SIZE, Chunk.RESOLUTION * y * Chunk.SIZE);
			for(int x = 0; x < SIZE; x++) {
				for(int y = 0; y < SIZE; y++) {
					CellType cell = new CellType(GetBiom(x, y), GetBiom(x+1, y), GetBiom(x, y+1), GetBiom(x+1, y+1));
					var key = tiles.GetCell(cell);
					SetCell(x, y, key.tileSet, autotileCoord: key.coord);
				}
			}
		}
	}

	private CornerType[] map = new CornerType[SIZE_PLUS_ONE * SIZE_PLUS_ONE];

	public void SetBiom(int x, int y, CornerType biom) {
		map[x + y * SIZE_PLUS_ONE] = biom;
	}

	public CornerType GetBiom(int x, int y) {
		return map[x + y * SIZE_PLUS_ONE];
	}
	public const int SIZE = 32;
	private const int SIZE_PLUS_ONE = SIZE + 1;
	public const int RESOLUTION = 24;

	public const int PIXEL_SIZE = SIZE * RESOLUTION;
}
