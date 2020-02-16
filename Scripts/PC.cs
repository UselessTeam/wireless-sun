using System;
using Godot;

public class PC : KinematicBody2D {

	public const float WALK_SPEED = 100;

	private bool isImpact = false;
	private float impactTime = 0;
	private Vector2 speed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () { }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (float delta) { }

	public override void _PhysicsProcess (float delta) {
		if (isImpact) { } else {
			speed = new Vector2 (0, 0);
			if (Input.IsActionPressed ("ui_up"))
				speed.y = -1;
			if (Input.IsActionPressed ("ui_down")) {
				speed.y = 1;
			}
			if (Input.IsActionPressed ("ui_left"))
				speed.x = -1;
			if (Input.IsActionPressed ("ui_right"))
				speed.x = 1;
			speed *= WALK_SPEED;
		}
		MoveAndCollide (speed * delta);
	}

}