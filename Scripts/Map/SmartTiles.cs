using Godot;
using System;
using System.Collections.Generic;

public struct CellType {
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
public enum Biom : byte {
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

	public Vector2 GetCell(CellType cell) {
		if (!tiles.ContainsKey(cell)) {
			return Vector2.Zero;
		} else {
			return tiles[cell][rnd.Next(tiles[cell].Count)];
		}
	}

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
	}
}
