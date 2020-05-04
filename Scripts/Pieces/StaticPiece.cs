using System;
using Godot;

public class StaticPiece : StaticBody2D, IPiece {
	public override void _Ready () {
		this.Ready ();
	}
}