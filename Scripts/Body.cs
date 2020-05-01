using System;
using Godot;

public class Body : KinematicBody2D {
	[Export] public float IMPACT_FACTOR = 800;
	[Export] public float WALK_SPEED = 100;

	public static Vector2 IsometricMultiplier = new Vector2 (1.5f, 1);

	[Signal] delegate void body_collision (KinematicCollision2D collInfo);
	[Signal] delegate void TakeDamage (AttackResource attack, Vector2 direction);
	KinematicCollision2D collInfo = null;
	public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
	public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

	public bool isImpact = false;
	private float impactTime = 0;
	private Vector2 impactDirection;

	[Puppet] private Vector2 facingDirection = new Vector2 (1, 0);
	[Master]
	public Vector2 FacingDirection {
		get { return facingDirection; }
		set {
			if (facingDirection == Vector2.Zero)
				return;
			facingDirection = value.Normalized ();
			if (Network.IsConnectionStarted)
				RsetUnreliable ("facingDirection", facingDirection);
		}
	}

	[Puppet] private Vector2 nextMovement = new Vector2 (0, 0);
	public Vector2 NextMovement {
		get { return nextMovement; }
		set {
			nextMovement = value;
			if (IsTrueMaster)
				RsetUnreliable ("nextMovement", nextMovement);
		}
	}

	[Puppet] Vector2 PuppetPosition = new Vector2 (0, 0);

	public bool CanMove { get { return !isImpact; } }

	public override void _Ready () {
		AddToGroup ("ReloadOnSave");
	}

	// Call whenever the body is hit for some time
	public void StartImpact (Vector2 direction, float time) {
		isImpact = true;
		impactTime = time;
		impactDirection = direction * IMPACT_FACTOR;
	}

	public override void _PhysicsProcess (float delta) {
		if (isImpact) { // Movement in case of impact
			impactTime -= delta;
			collInfo = MoveAndCollide (impactDirection * IsometricMultiplier * delta);
			if (impactTime <= 0) {
				impactTime = 0;
				isImpact = false;
			}
		} else if (NextMovement != Vector2.Zero) { // Normal Movement
			collInfo = MoveAndCollide (NextMovement * IsometricMultiplier * WALK_SPEED * delta);
		}
		if (IsMaster) {
			NextMovement = new Vector2 (0, 0);
			if (collInfo != null)
				EmitSignal ("body_collision", collInfo);
		}
		if (IsTrueMaster) //Master Code
			RsetUnreliable ("PuppetPosition", Position);
		else { //Puppet Code
			if (PuppetPosition != Vector2.Zero)
				Position = PuppetPosition;
		}

	}

	public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		GetChild ((int) saveObject["ControlPosition"]).Call ("LoadData", saveObject);
	}

}