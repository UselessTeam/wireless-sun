using Godot;
using System;
using System.Collections.Generic;

public struct CellType {
	public CornerType top_left;
	public CornerType top_right;
	public CornerType bottom_left;
	public CornerType bottom_right;

	public bool NoVoid() {
		return (top_left != CornerType.Void)
			&& (top_right != CornerType.Void)
			&& (bottom_left != CornerType.Void)
			&& (bottom_right != CornerType.Void);
	}
	
	public bool NoVoidOnly() {
		return (top_left != CornerType.Void)
			|| (top_right != CornerType.Void)
			|| (bottom_left != CornerType.Void)
			|| (bottom_right != CornerType.Void);
	}

	public CellType(CornerType top_left, CornerType top_right, CornerType bottom_left, CornerType bottom_right) {
		this.top_left = top_left;
		this.top_right = top_right;
		this.bottom_left = bottom_left;
		this.bottom_right = bottom_right;
	}

	public override String ToString() {
		return "[" + top_left + "," + top_right + "," + bottom_left + "," + bottom_right + "]";
	}
}
public enum CornerType : byte {
	Void = 0,
	Sea = 1,
	Sand = 2,
	Grass = 3,
	Stone = 4,
}
public class SmartTiles : TileMap
{
	private const uint TOP_LEFT = 1;
	private const uint TOP_RIGHT = 4;

	private const uint CENTER = 16;
	private const uint BOTTOM_LEFT = 64;
	private const uint BOTTOM_RIGHT = 256;

	private CornerType[] layers = { CornerType.Void, CornerType.Sea, CornerType.Sand, CornerType.Grass, CornerType.Stone };

	private Vector2 size;

	private Dictionary<CellType, List<TileKey>> raw_tiles = new Dictionary<CellType, List<TileKey>>();
	
	[Export]
	public TileSet world_tileset;
	private Dictionary<CellType, TileKey> built_tiles = new Dictionary<CellType, TileKey>();

	private void AddRawCell(CellType cell, TileKey id) {
		if (!raw_tiles.ContainsKey(cell)) {
			raw_tiles.Add(cell, new List<TileKey> {id});
		} else {
			raw_tiles[cell].Add(id);
		}
	}

	private Builder builder = null;
	private void AddBuiltCell(CellType cell) {
		if(builder == null) {
			builder = new Builder(this);
		}
		TileKey key = builder.BuiltCell(cell);
		built_tiles.Add(cell, key);
	}

	public struct TileKey {
		public byte tileSet;
		public byte x;
		public byte y;
		public Vector2 coord { get { return new Vector2(x,y); } }

		public TileKey(byte tileSet, byte x, byte y) {
			this.tileSet = tileSet;
			this.x = x;
			this.y = y;
		}

		public static readonly TileKey Null = new TileKey(byte.MaxValue, 0, 0);
	}

	static Random rnd = new Random();

	public TileKey GetCell(CellType cell) {
		// Check RawTiles
		if (raw_tiles.ContainsKey(cell)) {
			return raw_tiles[cell][rnd.Next(raw_tiles[cell].Count)];
		}
		if (!built_tiles.ContainsKey(cell)) {
			AddBuiltCell(cell);
		}
		return built_tiles[cell];
	}

	public override void _Ready()
	{
		size = TileSet.AutotileGetSize(0);
		for(byte x = 0; x < size.x; x++) {
			for(byte y = 0; y < size.y; y++) {
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
				if (cell.NoVoidOnly()) {
					AddRawCell(cell, new TileKey(0, x, y));			
				}
			}
		}
	}

	internal class Builder {

		// Ref
		private Image world_image;
		private TileSet world_tileset;

		private Dictionary<CellType, List<TileKey>> raw_tiles;

		// Counter
		private Image currentImage = null;
		private ImageTexture currentImageTexture = null;
		private byte currentTile = 0;
		private byte currentX = 0;
		private byte currentY = 0;
		private const byte RESOLUTION = 24;

		private const byte SIZE = 64;

		public Builder(SmartTiles smartTiles) {
			this.world_tileset = smartTiles.world_tileset;
			this.world_image = smartTiles.world_tileset.TileGetTexture(0).GetData();
			this.raw_tiles = smartTiles.raw_tiles;
		}

		private void CreateExtraTilemap() {
			currentImage = new Image();
			currentImage.Create(RESOLUTION * SIZE, RESOLUTION * SIZE, false, Image.Format.Rgb8);
			currentImage.Fill(new Color(1f,1f,1f));
			currentImageTexture = new ImageTexture();
			currentImageTexture.CreateFromImage(currentImage);
			currentTile += 1;
			world_tileset.CreateTile(currentTile);
			world_tileset.TileSetName(currentTile, "extra");
			world_tileset.TileSetTileMode(currentTile, TileSet.TileMode.AtlasTile);
			world_tileset.TileSetRegion(currentTile, new Rect2(0, 0, RESOLUTION * SIZE, RESOLUTION * SIZE));
			world_tileset.AutotileSetSize(currentTile, new Vector2(RESOLUTION, RESOLUTION));
			world_tileset.TileSetTexture(currentTile, currentImageTexture);
		}

		internal TileKey BuiltCell(CellType cell) {
			world_image.Lock();
			if(currentImage==null) {
				CreateExtraTilemap();
			}
			currentImage.Lock();
			var top_left = raw_tiles[new CellType(cell.top_left, CornerType.Void, CornerType.Void, CornerType.Void)][0];
			var top_right = raw_tiles[new CellType(CornerType.Void, cell.top_right, CornerType.Void, CornerType.Void)][0];
			var bottom_left = raw_tiles[new CellType(CornerType.Void, CornerType.Void, cell.bottom_left, CornerType.Void)][0];
			var bottom_right = raw_tiles[new CellType(CornerType.Void, CornerType.Void, CornerType.Void, cell.bottom_right)][0];
			List<TileKey> tile_list = new List<TileKey> {top_left, top_right, bottom_left, bottom_right};
			for(byte x = 0; x < RESOLUTION; x++) {
				for(byte y = 0; y < RESOLUTION; y++) {
					var color = Color.Color8(byte.MaxValue, 0, 0);
					foreach(var tile in tile_list) {
						var tile_color = world_image.GetPixel(RESOLUTION*tile.x + x, RESOLUTION*tile.y + y);
						if(tile_color.a > 0) {
							color = tile_color;
							break;
						}
					}
					currentImage.SetPixel(RESOLUTION*currentX + x, RESOLUTION*currentY + y, color);
				}
			}
			currentImageTexture.SetData(currentImage);
			world_tileset.TileSetTexture(currentTile, currentImageTexture);
			world_image.Unlock();
			currentImage.Unlock();
			TileKey key = new TileKey(currentTile, currentX, currentY);
			IncrementCounter();
			return key;
		}

		private void IncrementCounter() {
			currentX += 1;
			if(currentX > SIZE) {
				currentX = 0;
				currentY += 1;
				if(currentY > SIZE) {
					currentY = 0;
					currentImage = null;
				}
			}
		}
	}
}
