using System;
using Godot;

public class PC : Node2D {
	public const float WALK_SPEED = 100;

	Body myBody;
	[Puppet] Vector2 PuppetPosition = new Vector2 (0, 0);

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
			GetNode<PlayerAttack> ("../Attack")._StartAttack ();
			isAttacking = true;
		}

	}

	public override void _PhysicsProcess (float delta) {
		if (!Network.isConnectionStarted || IsNetworkMaster ()) { //Master Code
			var direction = new Vector2 (0, 0);
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

				if (Network.isConnectionStarted)
					Rset ("PuppetPosition", myBody.Position);
			} else { }
			var collInfo = myBody.MoveAndCollide (direction * WALK_SPEED * delta);
		} else { //Puppet Code
			if (PuppetPosition.LengthSquared () != 0)
				myBody.Position = PuppetPosition;

		}
	}

}