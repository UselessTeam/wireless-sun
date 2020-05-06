using System;
using Godot;

public interface IPiece {
	int GetZOffset();
}

public static class PieceExtensions {
	public static void PieceReady (this IPiece piece) {
		Node2D p = ((Node2D)piece);
		if (p.GetNodeOrNull ("Control") != null)
			p.AddToGroup ("ReloadOnSave");
		piece.SetZIndex ();
	}

	public static void LoadData (this IPiece piece, Godot.Collections.Dictionary<string, object> saveObject) {
		Node2D p = ((Node2D)piece);
		if (p.GetNodeOrNull ("Control") != null)
			p.GetNode<ControlComponent> ("Control").Call (nameof (ControlComponent.LoadData), saveObject);
		piece.SetZIndex ();
	}

	public static void SetZIndex (this IPiece piece) {
		Node2D p = ((Node2D)piece);
		p.ZIndex = Tile.TransposeDepth(p.Position.y) + piece.GetZOffset();
	}
}
