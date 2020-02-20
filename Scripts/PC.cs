using System;
using Godot;

public class PC : Node2D {
	public bool isAttacking = false;
	[RemoteSync] public Vector2 inputDirection;

	Body myBody { get { return GetParent<Body> (); } }
	public bool CanMove { get { return !isAttacking && myBody.CanMove; } }
	public bool isMaster { get { return !Network.isConnectionStarted || IsNetworkMaster (); } }

	public override void _Ready () { }

	public override void _Input (InputEvent @event) {
		if (isMaster)
			if (Network.isConnectionStarted)
				// If connected to a server, the event will be sent to all puppets
				this.Rpc ("_SendAction", @event);
			else
				// If not it will just be performed normally
				_Action (@event);
	}

	public void _Action (InputEvent @event) {
		if (@event.IsActionPressed ("ui_select") && CanMove) { // Action : Attack
			GetNode<PlayerAttack> ("../Attack")._StartAttack ();
			isAttacking = true;
		}
	}

	public override void _Process (float delta) {
		if (isMaster && CanMove) { // Master Code
			if (Input.IsActionPressed ("ui_up"))
				inputDirection.y = -1;
			if (Input.IsActionPressed ("ui_down"))
				inputDirection.y = 1;
			if (Input.IsActionPressed ("ui_left"))
				inputDirection.x = -1;
			if (Input.IsActionPressed ("ui_right"))
				inputDirection.x = 1;
			inputDirection = inputDirection.Normalized ();
			if (Network.isConnectionStarted)
				RsetUnreliable ("direction", inputDirection);
		}
		if (inputDirection.LengthSquared () > 0) {
			myBody.NextMovement = inputDirection;
			inputDirection = new Vector2 (0, 0);
		}
	}

	public override void _PhysicsProcess (float delta) { }

	[RemoteSync]
	public void _SendAction (InputEvent @event) { _Action (@event); }

}