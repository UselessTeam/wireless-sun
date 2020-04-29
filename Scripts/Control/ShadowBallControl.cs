using System;
using Godot;

public class ShadowBallControl : ControlComponent {
	[Export] public float FLICKER_TIME = 3;

	FieldOfView myFOV {
		get { return GetNode<FieldOfView> ("FieldOfView"); }
	}

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

	public new Godot.Collections.Dictionary<string, object> MakeSave () {
		var saveObject = base.MakeSave ();
		saveObject["HP"] = GetNode<HealthComponent> ("../Health").HP;
		return saveObject;
	}

	public new void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		base.LoadData (saveObject);
		GetNode<HealthComponent> ("../Health").HP = Convert.ToSingle (saveObject["HP"]);
	}
}
