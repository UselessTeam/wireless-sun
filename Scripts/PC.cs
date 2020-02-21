using System;
using Godot;

public class PC : _EnemyControl {
	public bool isAttacking = false;

	Body myBody { get { return GetParent<Body> (); } }
	public bool CanMove { get { return !isAttacking && myBody.CanMove; } }

	public override void _Ready () { }

	string[] ActionList = {
		"action_1" // Attack
	};

	public override void _Input (InputEvent _event) {
		if (isMaster) {
			foreach (var act in ActionList) {
				if (_event.IsActionPressed (act)) {
					if (Network.isConnectionStarted) {
						// If connected to a server, the event will be sent to all puppets
						this.Rpc ("_Action", act);
						_Action (act);
					} else
						// If not it will just be performed normally
						_Action (act);
					return;
				}
			}
			if (_event is InputEventMouseMotion eventMouseMotion) {
				myBody.FacingDirection = GetGlobalMousePosition () - myBody.Position;
			}
		}
	}

	[Puppet] public void _Action (string _event) {
		if (_event == ActionList[0]) { // Action : Attack
			GetNode<PlayerAttack> ("../Attack")._StartAttack ();
			isAttacking = true;
		}
	}

	public override void _Process (float delta) {
		if (isMaster && CanMove) { // Master Code
			Vector2 inputMovement = new Vector2 (0, 0);
			if (Input.IsActionPressed ("ui_up"))
				inputMovement.y = -1;
			if (Input.IsActionPressed ("ui_down"))
				inputMovement.y = 1;
			if (Input.IsActionPressed ("ui_left"))
				inputMovement.x = -1;
			if (Input.IsActionPressed ("ui_right"))
				inputMovement.x = 1;
			if (inputMovement != Vector2.Zero) {
				inputMovement = inputMovement.Normalized ();
				myBody.NextMovement = inputMovement;
				inputMovement = new Vector2 (0, 0);
			}
		}
	}

	public override void _PhysicsProcess (float delta) { }

}