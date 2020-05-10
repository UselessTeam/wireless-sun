using System;
using System.Collections.Generic;
using Godot;

public class BuildingComponent : Node2D {
    private static string LAYOUT_TEMPLATE = @"
XXXXXX
X    X
X    X
XXX XX".TrimStart ('\n');

    // Fill height, width, and layout out of string such a LAYOUT_TEMPLATE
    private void LoadLayoutString (string layoutString) {
        List<List<bool>> buildingLines = new List<List<bool>> ();
        foreach (string line in layoutString.Split ('\n')) {
            List<bool> buildingLine = new List<bool> ();
            foreach (char p in line) {
                buildingLine.Add (p != ' ');
            }
            width = Math.Max (width, buildingLine.Count);
            buildingLines.Add (buildingLine);
        }
        height = buildingLines.Count;
        layout = new bool[width * height];
        int v = 0;
        foreach (List<bool> buildingLine in buildingLines) {
            int u = 0;
            foreach (bool cell in buildingLine) {
                SetWall (u, v, cell);
                u++;
            }
            v++;
        }
    }

    private Node2D walls;
    private Node2D roof;

    public int width { get; private set; }
    public int height { get; private set; }

    private bool[] layout;

    public override void _Ready () {
        walls = GetNode<Node2D> ("Walls");
        roof = GetNode<Node2D> ("../Display/Roof");
        LoadLayoutString (LAYOUT_TEMPLATE);
        for (int u = 0; u < width; u++) {
            for (int v = 0; v < height; v++) {
                if (HasWall (u, v)) {
                    walls.AddChild (Tile.Instance (u, v, 3, TileType.BRICK));
                }
                if (HasRoof (u, v)) {
                    roof.AddChild (Tile.Instance (u, v, 4, TileType.ROOF));
                }
            }
        }
    }

    public bool HasWall (int u, int v) {
        if (u < 0 || v < 0 || u >= width || v >= height) {
            return false;
        }
        return layout[width * v + u];
    }

    public bool HasRoof (int u, int v) {
        if (u < 0 || v < 0 || u >= width || v >= height) {
            return false;
        }
        return true;
    }

    private void SetWall (int u, int v, bool value) {
        layout[width * v + u] = value;
    }
}
