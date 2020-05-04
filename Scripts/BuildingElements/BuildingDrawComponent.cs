using Godot;
using System;

public class BuildingDrawComponent : TileMap
{
	private SmartBuildingTiles tiles;
	private BuildingComponent building;
	public bool modified = true;
	public override void _Ready()
	{
		tiles = (SmartBuildingTiles)GetNode("/root/SmartTiles/Walls");
		building = (BuildingComponent)GetNode("../Building");
		if(building == null) {
			GD.PrintErr("BuildingDraw has not BuildingComponent");
		}
	}

	public static string layout =@"
XXXXXX
X    X
X    X
XXX XX";

	public bool HasWall(int x, int y) {
		return (x>=0) && (x<X) && (y>=0) && (y<Y) &&
			((x==0) || (x==X-1) || (y == 0) | (y == Y-1 && x!=2));
	}

	private static int X = 6;
	private static int Y = 4;

	public override void _Draw() {
		if(modified) {
			modified = false;
			for(int x = 0; x < X; x++) {
				for(int y = 0; y < Y; y++) {
					if (HasWall(x,y)) {
						MetaTile.BuildingType cell = new MetaTile.BuildingType(
							HasWall(x+1, y), HasWall(x, y-1), HasWall(x-1, y), HasWall(x, y+1)
						);
						var key = tiles.GetCell(cell);
						key.Apply(x, y, this);
					}
				}
			}
		}
	}
}
