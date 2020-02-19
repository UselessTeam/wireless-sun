using System;
using Godot;

public class PC : Node2D {
	public const float WALK_SPEED = 100;

	public bool isAttacking = false;
	[RemoteSync] public Vector2 direction;

	Body myBody { get { return GetParent<Body> (); } }
	public bool canMove { get { return !isAttacking && myBody.CanMove (); } }
	public bool isMaster { get { return !Network.isConnectionStarted || IsNetworkMaster (); } }

	public override void _Ready () { }

	public override void _Input (InputEvent @event) {
		if (@event.IsActionPressed ("ui_select") && canMove) {
			if (isMaster)
				if (Network.isConnectionStarted)
					this.Rpc ("_SendAttack");
				else
					_Attack ();
		}
	}

	public override void _Process (float delta) {
		if (isMaster) {
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
			if (Network.isConnectionStarted)
				RsetUnreliable ("direction", direction);
		}
		if (direction.Dot (direction) != 0)
			myBody.facingDirection = direction;
	}

	public override void _PhysicsProcess (float delta) {
		if (canMove) {
			var collInfo = myBody.MoveAndCollide (direction * WALK_SPEED * delta);
			direction = new Vector2 (0, 0);
		} else { }
	}

	[RemoteSync]
	public void _SendAttack () { _Attack (); }

	public void _Attack () {
		GetNode<PlayerAttack> ("../Attack")._StartAttack ();
		isAttacking = true;
	}
}