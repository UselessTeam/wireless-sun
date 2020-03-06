using Godot;
using System;
using System.Collections.Generic;
using MetaTile;
public class SmartWorldTiles : TileMap
{
	private const uint TOP_LEFT = 1;
	private const uint TOP_RIGHT = 4;

	private const uint CENTER = 16;
	private const uint BOTTOM_LEFT = 64;
	private const uint BOTTOM_RIGHT = 256;

	private PointType[] layers = { PointType.Void, PointType.Sea, PointType.Sand, PointType.Grass, PointType.Stone };

	private Vector2 size;

	public Dictionary<CellType, List<TileKey>> tiles = new Dictionary<CellType, List<TileKey>>();
	
	[Export]
	public TileSet world_tileset;
	private Dictionary<FragmentType, CornerKey> fragments = new Dictionary<FragmentType, CornerKey>();

	private void AddRawCell(CellType cell, TileKey id) {
		if (!tiles.ContainsKey(cell)) {
			tiles.Add(cell, new List<TileKey> {id});
		} else {
			tiles[cell].Add(id);
		}
	}

	private Builder builder = null;
	static Random rnd = new Random();

	public TileKey GetCell(CellType cell) {
		// Check RawTiles if the whole tile already exists
		if (tiles.ContainsKey(cell)) {
			return tiles[cell][rnd.Next(tiles[cell].Count)];
		}
		if(builder == null) {
			builder = new Builder(this);
		}
		// Shatter the tiles into 8 triangles, build or find every triangle, combine them in pairs for each corners, and send the resulting tile
		CornerKey[] corners = new CornerKey[4]; // top left, top right, bottom left, bottom right
		int i = 0;
		foreach(FragmentType[] fragmentPair in cell.Shatter()) {
			foreach(FragmentType fragment in fragmentPair) {
				if (!fragments.ContainsKey(fragment)) {
					fragments.Add(fragment, builder.BuildFragment(fragment));
				}
			}
			corners[i] = fragments[fragmentPair[0]] + fragments[fragmentPair[1]];
			i++;
		}
		return new TileKey(corners);
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
					if(layers[l]==PointType.Void && (bitmask & CENTER) != 0) {
						cell.middle_hole = true;
					}
				}
				if (!cell.NoVoid()) {
					cell.middle_hole = true;
				}
				if (cell.NoVoidOnly()) {
					AddRawCell(cell, new TileKey(0, x, y));			
				}
			}
		}
	}
}
