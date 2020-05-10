using System;
using Godot;

public class KinematicPiece : KinematicBody2D, IPiece {
    public override void _Ready () {
        this.PieceReady ();
    }
        
    [Export]
    public int zOffset = 1;
    public int GetZOffset() {
        return zOffset;
    }
}
