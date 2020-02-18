using System;
using Godot;

public class PC : Node2D {
	public const float WALK_SPEED = 100;

	Body myBody;

	public bool isAttacking = false;

	public bool CanMove () {
		return !isAttacking && myBody.CanMove ();
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		myBody = GetParent<Body> ();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (float delta) {
		if (Input.IsActionJustPressed ("ui_select") && CanMove ()) {
			GetNode<Attack> ("Attack")._StartAttack ();
			isAttacking = true;
		}

	}

	public override void _PhysicsProcess (float delta) {
		var direction = new Vector2 (0, 0);
		// GD.Print (GetNode<KinematicBody2D> ("Body"));
		if (CanMove ()) {
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
				myBody.facingDirection = direction;
		} else { }
		var collInfo = myBody.MoveAndCollide (direction * WALK_SPEED * delta);

	}

}