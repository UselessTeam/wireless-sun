using System;
using Godot;

public interface IPiece {}

public static class PieceExtensions {
    public static void Ready (this IPiece piece) {
        if ((piece as Node2D).GetNodeOrNull ("Control") != null)
            (piece as Node2D).AddToGroup ("ReloadOnSave");
    }
    public static void LoadData (this IPiece piece, Godot.Collections.Dictionary<string, object> saveObject) {
        if ((piece as Node2D).GetNodeOrNull ("Control") != null)
            (piece as Node2D).GetNode<ControlComponent> ("Control").Call (nameof (ControlComponent.LoadData), saveObject);
        piece.SetZAxis ();
    }

    public static void SetZAxis (this IPiece piece) {
        (piece as Node2D).ZIndex = (int) ((piece as Node2D).GlobalPosition.y);
    }

}