using Godot;
using System;
using MetaTile;

namespace MetaTile {
	[Flags]
	public enum BuildingConnection : byte {
		Right = 1,
		Top = 2,
		Left = 4,
		Bottom = 8,

		None = 0,
		TopLeft = Top | Left,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right,

	}

	public struct BuildingType {
		public BuildingConnection connections;

		// TODO: Add wall/floor type
		public BuildingType(bool right, bool top, bool left, bool bottom) {
			this.connections = BuildingConnection.None;
			if(right) {
				this.connections |= BuildingConnection.Right;
			}
			if(top) {
				this.connections |= BuildingConnection.Top;
			}
			if(left) {
				this.connections |= BuildingConnection.Left;
			}
			if(bottom) {
				this.connections |= BuildingConnection.Bottom;
			}
		}
	}
}
public class SmartBuildingTiles : TileMap
{
	public TileKey GetCell(BuildingType cell) {
		CornerKey[] corners = new CornerKey[4];
		
		BuildingConnection tl = cell.connections & BuildingConnection.TopLeft;
		if (tl == BuildingConnection.None) {
			corners[0] = new CornerKey(0,0,2);
		} else if (tl == BuildingConnection.Top) {
			corners[0] = new CornerKey(0,0,4);
		} else if (tl == BuildingConnection.Left) {
			corners[0] = new CornerKey(0,2,2);
		} else {
			corners[0] = new CornerKey(0,2,0);
		}

		BuildingConnection tr = cell.connections & BuildingConnection.TopRight;
		if (tr == BuildingConnection.None) {
			corners[1] = new CornerKey(0,3,2);
		} else if (tr == BuildingConnection.Top) {
			corners[1] = new CornerKey(0,3,4);
		} else if (tr == BuildingConnection.Right) {
			corners[1] = new CornerKey(0,1,2);
		} else {
			corners[1] = new CornerKey(0,3,0);
		}

		BuildingConnection bl = cell.connections & BuildingConnection.BottomLeft;
		if (bl == BuildingConnection.None) {
			corners[2] = new CornerKey(0,0,5);
		} else if (bl == BuildingConnection.Bottom) {
			corners[2] = new CornerKey(0,0,3);
		} else if (bl == BuildingConnection.Left) {
			corners[2] = new CornerKey(0,2,5);
		} else {
			corners[2] = new CornerKey(0,2,1);
		}

		BuildingConnection br = cell.connections & BuildingConnection.BottomRight;
		if (br == BuildingConnection.None) {
			corners[3] = new CornerKey(0,3,5);
		} else if (br == BuildingConnection.Bottom) {
			corners[3] = new CornerKey(0,3,3);
		} else if (br == BuildingConnection.Right) {
			corners[3] = new CornerKey(0,1,5);
		} else {
			corners[3] = new CornerKey(0,3,1);
		}

		return new TileKey(corners);
	}
	public override void _Ready()
	{
		
	}
}
