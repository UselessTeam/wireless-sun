using System;
using Godot;

public class PC : Node2D {

	public bool isAttacking = false;
	private Vector2 facingDirection;

	public bool IsCanMove () {
		return !isAttacking && !(GetParent<Body> ().isImpact);
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
		// GD.Print (GetNode<KinematicBody2D> ("Body"));
		if (IsCanMove ()) {
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
		} else { }
		GetParent<Body> ().Walk (direction, delta);
	}

}