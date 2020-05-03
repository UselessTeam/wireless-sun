using System;
using Godot;

public class KinematicPiece : KinematicBody2D, IPiece {
	public override void _Ready () {
		AddToGroup ("ReloadOnSave");
	}

	public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		GetNode<ControlComponent> ("Control").Call (nameof (ControlComponent.LoadData), saveObject);
	}

}