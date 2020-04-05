using System;
using Godot;

public class ShadowBallControl : _Control {
	[Export] public float FLICKER_TIME = 3;

	FieldOfView myFOV {
		get { return GetNode<FieldOfView> ("FieldOfView"); }
	}

	public override void _Ready () { }

	public bool CanSeePlayer {
		get { return myFOV.IsPlayerDetected (); }
	}

	public override void _PhysicsProcess (float delta) {
		if (IsMaster) {
			var direction = new Vector2 (0, 0);
			if (CanSeePlayer && CanMove) {
				var playerBody = myFOV.GetClosestPlayer ();
				direction = (playerBody.GlobalPosition - MyBody.GlobalPosition).Normalized ();
				MyBody.NextMovement = direction;
			}
		}
	}

	public override void _OnDamageTaken (float damage) {
		MyBody.StartFlicker (FLICKER_TIME);
	}

	public override void SaveIn (Godot.Collections.Dictionary<string, object> saveObject) {
		saveObject["HP"] = GetNode<Health> ("Health").HP;
	}

	public override void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		GetNode<Health> ("Health").HP = (float) saveObject["HP"];
	}
}