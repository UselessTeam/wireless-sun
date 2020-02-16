using Godot;
using System;

public class ChangableSprite : Sprite
{
	[Export]
	private int firstIndex;
	[Export]
	private int totalIndex;
	
	[Signal]
	delegate void changed(int to);

	public void Change(int to) {
		EmitSignal(nameof(changed), to);
		Frame = firstIndex + to;
	}

	public void RandomSetup() {
		int new_value = General.rng.Next(totalIndex);
		Change(new_value);
	}
}
