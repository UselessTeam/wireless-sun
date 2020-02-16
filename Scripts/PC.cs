using System;
using Godot;

public class PC : KinematicBody2D {

	private bool isAttacking = false;
	private float impactTime = 0;
	private Vector2 facingDirection;

	public bool IsCanMove () {
		return !isAttacking && !GetNode<Body> ("Body").isImpact;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready () { }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (float delta) {
		if (Input.IsActionJustPressed ("ui_select") && IsCanMove ()) {
			GetNode<Attack> ("Attack")._StartAttack (facingDirection);
			isAttacking = true;
		}

	}

	public override void _PhysicsProcess (float delta) {
		Vector2 direction = new Vector2 (0, 0);
		if (IsCanMove ()) { } else {
			if (Input.IsActionPressed ("ui_up"))
				direction.y = -1;
			if (Input.IsActionPressed ("ui_down")) {
				direction.y = 1;
			}
			if (Input.IsActionPressed ("ui_left"))
				direction.x = -1;
			if (Input.IsActionPressed ("ui_right"))
				direction.x = 1;
			direction = direction.Normalized ();
			if (direction.Dot (direction) != 0)
				facingDirection = direction;
		}
		GetNode<Body> ("Body").Walk (direction, delta);
	}

}