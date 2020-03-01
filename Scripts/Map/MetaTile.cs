using Godot;
using System;
using System.Collections.Generic;

namespace MetaTile {

	public static class TILE {
		public const byte RESOLUTION = 24;
		public const byte HALF_RESOLUTION = RESOLUTION / 2;
	}

	// Keys
	// Used to uniquely identify how to find a tile texture
	public struct CornerKey {
		public byte tileSet;
		public byte x;
		public byte y;

		public bool otherLayer;
		public byte otherTileSet;
		public byte otherX;
		public byte otherY;
		
		public Vector2 coord { get { return new Vector2(x,y); } }
		public Vector2 otherCoord { get { return new Vector2(otherX,otherY); } }
		public CornerKey(byte tileSet, byte x, byte y) {
			this.tileSet = tileSet;
			this.x = x;
			this.y = y;
			this.otherLayer = false;
			this.otherTileSet = 0;
			this.otherX = 0;
			this.otherY = 0;
		}

		public CornerKey(byte tileSet, byte x, byte y, byte otherTileSet, byte otherX, byte otherY) {
			this.tileSet = tileSet;
			this.x = x;
			this.y = y;
			this.otherLayer = true;
			this.otherTileSet = otherTileSet;
			this.otherX = otherX;
			this.otherY = otherY;
		}

		public static CornerKey operator +(CornerKey a, CornerKey b) {
			return new CornerKey(a.tileSet, a.x, a.y, b.tileSet, b.x, b.y);
		}

		public void Apply(Chunk chunk, int x, int y) {
			chunk.SetCell(x, y, tileSet, autotileCoord: coord);
			if(otherLayer) {
				chunk.otherLayer.SetCell(x, y, otherTileSet, autotileCoord: otherCoord);
			}
		}
	}

	public struct TileKey {

		public CornerKey[] corners;

		public TileKey(byte tileSet, byte x, byte y) {
			this.corners = new CornerKey[] {
				new CornerKey(tileSet, (byte)(2*x), (byte)(2*y)),
				new CornerKey(tileSet, (byte)(2*x+1), (byte)(2*y)),
				new CornerKey(tileSet, (byte)(2*x), (byte)(2*y+1)),
				new CornerKey(tileSet, (byte)(2*x+1), (byte)(2*y+1))
			};
		}
		public TileKey(CornerKey[] corners) {
			this.corners = corners;
		}

		public void Apply(Chunk chunk, int x, int y) {
			this.corners[0].Apply(chunk, 2*x, 2*y);
			this.corners[1].Apply(chunk, 2*x+1, 2*y);
			this.corners[2].Apply(chunk, 2*x, 2*y+1);
			this.corners[3].Apply(chunk, 2*x+1, 2*y+1);
		}

		public static readonly TileKey Null = new TileKey(byte.MaxValue, 0, 0);
	}

	// Types
	// Represents different layout of Grass/Sand/etc...
	public struct CellType {
		public PointType top_left;
		public PointType top_right;
		public PointType bottom_left;
		public PointType bottom_right;

		public bool middle_hole;

		public bool NoVoid() {
			return (top_left != PointType.Void)
				&& (top_right != PointType.Void)
				&& (bottom_left != PointType.Void)
				&& (bottom_right != PointType.Void);
		}
		
		public bool NoVoidOnly() {
			return (top_left != PointType.Void)
				|| (top_right != PointType.Void)
				|| (bottom_left != PointType.Void)
				|| (bottom_right != PointType.Void);
		}

		public CellType(PointType top_left, PointType top_right, PointType bottom_left, PointType bottom_right) {
			this.top_left = top_left;
			this.top_right = top_right;
			this.bottom_left = bottom_left;
			this.bottom_right = bottom_right;
			this.middle_hole = false;
			if(!NoVoid()) {
				middle_hole = true;
			}
		}

		public CellType(PointType top_left, PointType top_right, PointType bottom_left, PointType bottom_right, bool middle_hole) {
			this.top_left = top_left;
			this.top_right = top_right;
			this.bottom_left = bottom_left;
			this.bottom_right = bottom_right;
			this.middle_hole = middle_hole;
		}

		public override String ToString() {
			return "[" + top_left + "," + top_right + "," + bottom_left + "," + bottom_right + "]";
		}

		public FragmentType[][] Shatter() {
			return new FragmentType[][] {
				new FragmentType[] {
					new FragmentType(FragmentType.TOP_LEFT, top_left, top_right),
					new FragmentType(FragmentType.LEFT_TOP, top_left, bottom_left),
				},
				new FragmentType[] {
					new FragmentType(FragmentType.TOP_RIGHT, top_right, top_left),
					new FragmentType(FragmentType.RIGHT_TOP, top_right, bottom_right),
				},
				new FragmentType[] {
					new FragmentType(FragmentType.BOTTOM_LEFT, bottom_left, bottom_right),
					new FragmentType(FragmentType.LEFT_BOTTOM, bottom_left, top_left),
				},
				new FragmentType[] {
					new FragmentType(FragmentType.BOTTOM_RIGHT, bottom_right, bottom_left),
					new FragmentType(FragmentType.RIGHT_BOTTOM, bottom_right, top_right),
				},
			};
		}
	}

	// Fragments are sub-triangles of cells
	// A cell can be shattered into eight fragments
	public struct FragmentType {

		[Flags]
		public enum FragmentPosition : byte {
			Right = 1,
			Bottom = 2,
			ClockWise = 4,
		}


		public static readonly FragmentPosition LEFT_TOP = 0;
		public static readonly FragmentPosition TOP_LEFT = FragmentPosition.ClockWise;
		public static readonly FragmentPosition TOP_RIGHT = FragmentPosition.Right;
		public static readonly FragmentPosition RIGHT_TOP = FragmentPosition.Right | FragmentPosition.ClockWise;
		public static readonly FragmentPosition BOTTOM_LEFT = FragmentPosition.Bottom;
		public static readonly FragmentPosition LEFT_BOTTOM = FragmentPosition.Bottom | FragmentPosition.ClockWise;
		public static readonly FragmentPosition RIGHT_BOTTOM = FragmentPosition.Right | FragmentPosition.Bottom;
		public static readonly FragmentPosition BOTTOM_RIGHT = FragmentPosition.Right | FragmentPosition.Bottom | FragmentPosition.ClockWise;
		public FragmentPosition position;

		public PointType self;
		public PointType facing;

		public FragmentType(FragmentPosition position, PointType self, PointType facing) {
			this.position = position;
			this.self = self;
			this.facing = facing;
		}

		public CellType CrossExtend() {
			if((((byte)(position) & 2) != 0) == (((byte)(position) & 1) != 0) ) {
				return new CellType(self, facing, facing, self, true);
			}
			// RIGHT_BOTTOM
			return new CellType(facing, self, self, facing, true);
		}

		public CellType SideExtend() {
			if(position == TOP_LEFT) {
				return new CellType(self, facing, PointType.Void, PointType.Void);
			}
			if(position == LEFT_TOP) {
				return new CellType(self, PointType.Void, facing, PointType.Void);
			}
			if(position == TOP_RIGHT) {
				return new CellType(facing, self, PointType.Void, PointType.Void);
			}
			if(position == RIGHT_TOP) {
				return new CellType(PointType.Void, self, PointType.Void, facing);
			}
			if(position == BOTTOM_LEFT) {
				return new CellType(PointType.Void, PointType.Void, self, facing);
			}
			if(position == LEFT_BOTTOM) {
				return new CellType(facing, PointType.Void, self, PointType.Void);
			}
			if(position == BOTTOM_RIGHT) {
				return new CellType(PointType.Void, PointType.Void, facing, self);
			}
			// RIGHT_BOTTOM
			return new CellType(PointType.Void, facing, PointType.Void, self);
		}

		public CellType CornerExtend() {
			if(position == TOP_LEFT || position == LEFT_TOP) {
				return new CellType(self, PointType.Void, PointType.Void, PointType.Void);
			}
			if(position == TOP_RIGHT || position == RIGHT_TOP) {
				return new CellType(PointType.Void, self, PointType.Void, PointType.Void);
			}
			if(position == BOTTOM_LEFT || position == LEFT_BOTTOM) {
				return new CellType(PointType.Void, PointType.Void, self, PointType.Void);
			}
			// BOTTOM_RIGHT || RIGHT_BOTTOM
			return new CellType(PointType.Void, PointType.Void, PointType.Void, self);
		}

		public bool IsInside(byte x, byte y) {
			if(position == TOP_LEFT) {
				return x >= y;
			}
			if(position == LEFT_TOP) {
				return x <= y;
			}
			if(position == TOP_RIGHT) {
				return x + y <= TILE.HALF_RESOLUTION;
			}
			if(position == RIGHT_TOP) {
				return x + y >= TILE.HALF_RESOLUTION;
			}
			if(position == BOTTOM_LEFT) {
				return x + y >= TILE.HALF_RESOLUTION;
			}
			if(position == LEFT_BOTTOM) {
				return x + y <= TILE.HALF_RESOLUTION;
			}
			if(position == BOTTOM_RIGHT) {
				return x <= y;
			}
			// RIGHT_BOTTOM
			return x >= y;
		}
	}

	public enum PointType : byte {
		Void = 0,
		Sea = 1,
		Sand = 2,
		Grass = 3,
		Stone = 4,
	}

	// The builder can built fragments
	// TODO : The builder should also alter the color of the tile it builds
	public class Builder {
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
		private const byte SIZE = 64;

		public Builder(SmartTiles smartTiles) {
			this.world_tileset = smartTiles.world_tileset;
			this.world_image = smartTiles.world_tileset.TileGetTexture(0).GetData();
			this.raw_tiles = smartTiles.tiles;
		}

		private void CreateExtraTilemap() {
			currentImage = new Image();
			currentImage.Create(TILE.HALF_RESOLUTION * SIZE, TILE.HALF_RESOLUTION * SIZE, false, Image.Format.Rgba8);
			currentImage.Fill(new Color(1f,1f,1f));
			currentImageTexture = new ImageTexture();
			currentImageTexture.CreateFromImage(currentImage);
			currentTile += 1;
			world_tileset.CreateTile(currentTile);
			world_tileset.TileSetName(currentTile, "extra");
			world_tileset.TileSetTileMode(currentTile, TileSet.TileMode.AtlasTile);
			world_tileset.TileSetRegion(currentTile, new Rect2(0, 0, TILE.HALF_RESOLUTION * SIZE, TILE.HALF_RESOLUTION * SIZE));
			world_tileset.AutotileSetSize(currentTile, new Vector2(TILE.HALF_RESOLUTION, TILE.HALF_RESOLUTION));
			world_tileset.TileSetTexture(currentTile, currentImageTexture);
		}

		internal CornerKey BuildFragment(FragmentType fragment) {
			world_image.Lock();
			if(currentImage==null) {
				CreateExtraTilemap();
			}
			currentImage.Lock();
			TileKey key;
			List<TileKey> keylist;
			if(raw_tiles.TryGetValue(fragment.SideExtend(), out keylist)) {
				key = keylist[0];
			} else if(raw_tiles.TryGetValue(fragment.CrossExtend(), out keylist)) {
				key = keylist[0];
			} else if(raw_tiles.TryGetValue(fragment.CornerExtend(), out keylist)) {
				key = keylist[0];
			} else {
				throw new Exception("Couldn't find base for fragment "+fragment+", maybe the tileset was not properly configured, or Swynfel forgot to draw some tiles");
			}
			int corner_type = (byte)(fragment.position) & 3;
			CornerKey inputCorner = key.corners[corner_type];
			for(byte x = 0; x < TILE.HALF_RESOLUTION; x++) {
				for(byte y = 0; y < TILE.HALF_RESOLUTION; y++) {
					Color color;
					if (!fragment.IsInside(x,y)) {
						color = new Color(0,0,0,0);
					} else {
						color = world_image.GetPixel(TILE.HALF_RESOLUTION*inputCorner.x + x, TILE.HALF_RESOLUTION*inputCorner.y + y);
					}
					currentImage.SetPixel(TILE.HALF_RESOLUTION*currentX + x, TILE.HALF_RESOLUTION*currentY + y, color);
				}
			}
			currentImageTexture.SetData(currentImage);
			world_tileset.TileSetTexture(currentTile, currentImageTexture);
			world_image.Unlock();
			currentImage.Unlock();
			CornerKey cornerKey = new CornerKey(currentTile, currentX, currentY);
			IncrementCounter();
			return cornerKey;
		}

		private void IncrementCounter() {
			currentX += 1;
			if(currentX >= SIZE) {
				currentX = 0;
				currentY += 1;
				if(currentY >= SIZE) {
					currentY = 0;
					currentImage = null;
				}
			}
		}
	}
}