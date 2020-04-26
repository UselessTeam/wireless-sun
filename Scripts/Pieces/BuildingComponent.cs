using Godot;
using System;
using System.Collections.Generic;

public class BuildingComponent : Node2D
{
	private static string LAYOUT_TEMPLATE =@"
XXXXXX
X    X
X    X
XXX XX".TrimStart('\n');
	public int width { get; private set; }
	public int height { get; private set; }

	private bool[] layout;

	public override void _Ready()
	{
		List<List<bool>> buildingLines = new List<List<bool>>();
		foreach(string line in LAYOUT_TEMPLATE.Split('\n')) {
			List<bool> buildingLine = new List<bool>();
			foreach(char p in line) {
				buildingLine.Add(p != ' ');
			}
			width = Math.Max(width, buildingLine.Count);
			buildingLines.Add(buildingLine);
		}
		height = buildingLines.Count;
		layout = new bool[width * height];
		int y = 0;
		foreach(List<bool> buildingLine in buildingLines) {
			int x = 0;
			foreach(bool cell in buildingLine) {
				SetWall(x, y, cell);
				x++;
			}
			y++;
		}
	}

	public bool HasWall(int x, int y) {
		if (x<0 || y<0 || x>=width || y>=height ) {
			return false;
		}
		return layout[width*y + x];
	}

	public MetaTile.BuildingType? WallType(int x, int y) {
		if(!HasWall(x,y)) {
			return null;
		}
		return new MetaTile.BuildingType(
			HasWall(x+1, y), HasWall(x, y-1), HasWall(x-1, y), HasWall(x, y+1)
		);
	}


	public bool HasRoof(int x, int y) {
		if (x<0 || y<0 || x>=width || y>=height ) {
			return false;
		}
		return true;
	}

	public MetaTile.BuildingType? RoofType(int x, int y) {
		if(!HasRoof(x,y)) {
			return null;
		}
		return new MetaTile.BuildingType(
			HasRoof(x+1, y), HasRoof(x, y-1), HasRoof(x-1, y), HasRoof(x, y+1)
		);
	}

	private void SetWall(int x, int y, bool value) {
		layout[width*y + x] = value;
	}
}
