using System;
using Godot;

public interface IPiece {
    int GetZOffset ();
}

public static class PieceExtensions {
    public static void PieceReady (this IPiece piece) {
        Node2D p = ((Node2D) piece);
        if (p.GetNodeOrNull ("Control") != null)
            p.AddToGroup ("ReloadOnSave");
        piece.UpdateZ ();
    }

    public static void LoadData (this IPiece piece, Godot.Collections.Dictionary<string, object> saveObject) {
        Node2D p = ((Node2D) piece);
        if (p.GetNodeOrNull ("Control") != null)
            p.GetNode<ControlComponent> ("Control").Call (nameof (ControlComponent.LoadData), saveObject);
        piece.UpdateZ ();
    }

    public static void UpdateZ (this IPiece piece) {
        ((Node2D) piece).UpdateZNode (piece.GetZOffset ());
    }
    public static void UpdateZNode (this Node2D node2d, int offset = 0) {
        node2d.ZIndex = Tile.TransposeDepth (node2d.Position.y) + offset;
    }
}
