using Godot;
using System;

public class BuildingWalls : TileMap
{
	private SmartBuildingTiles tiles;
	private BuildingComponent building;
	public bool modified = true;
	public override void _Ready()
	{
		tiles = (SmartBuildingTiles)GetNode("/root/SmartTiles/Walls");
		building = (BuildingComponent)GetParent();
	}

	public override void _Draw() {
		if(modified) {
			modified = false;
			for(int x = 0; x < building.width; x++) {
				for(int y = 0; y < building.height; y++) {
					var cell = building.WallType(x, y);
					if(cell.HasValue){
						var key = tiles.GetCell(cell.Value);
						key.Apply(x, y, this);
					}
				}
			}
		}
	}
}
