using System;
using Godot;

public class PC : Node2D {

	public const float WALK_SPEED = 100;

	private bool isImpact = false;
	private float impactTime = 0;
	private Vector2 speed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		GD.Print ("Hi!")
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (float delta) {
		int a = 1 / 0;
		GD.Print ("Hi!");
	}

	public override void _PhysicsProcess (float delta) {
		GD.Print ("HIIII");
		if (isImpact) { } else {
			speed = Vector2 (0, 0);
			if (Input.IsActionPressed ("up"))
				speed.y = -1;
			if (Input.IsActionPressed ("down")) {
				speed.y = 1;
				GD.Print ("HIIII");
			}
			if (Input.IsActionPressed ("left"))
				speed.x = -1;
			if (Input.IsActionPressed ("right"))
				speed.x = 1;
			speed *= WALK_SPEED;
		}
		MoveAndCollide (speed * delta);
	}
}
