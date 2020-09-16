using System;
using Godot;

public class BodilessPiece : Area2D, IPiece {
	public override void _Ready () {
		this.PieceReady ();
	}
	
	[Export]
	public int zOffset = 0;
	public int GetZOffset() {
		return zOffset;
	}
}
