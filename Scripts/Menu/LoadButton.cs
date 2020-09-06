using System;
using Godot;

public class LoadButton : Button {
	[Signal] public delegate void LoadPressed ();
	public override void _Ready () {
		Connect ("pressed", this, nameof (_OnPressed));
	}
	public void _OnPressed () { EmitSignal (nameof (LoadPressed), Name); }
}
