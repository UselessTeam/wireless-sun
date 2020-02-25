using System.Collections;
using System.Collections.Generic;
using Godot;

public class Character : Node2D {
	public List<ChangableSprite> items;
	public override void _Ready () {
		items = GetNode ("Body").GetChildren<ChangableSprite> ();
		RandomSetup ();
	}

	public void RandomSetup () {
		foreach (var item in items) {
			item.RandomSetup ();
		}
	}
}
