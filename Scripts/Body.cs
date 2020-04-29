using System;
using Godot;

public class Body : KinematicBody2D {
	[Export] public float IMPACT_FACTOR = 800;
	[Export] public float WALK_SPEED = 100;

	[Signal] delegate void body_collision (KinematicCollision2D collInfo);
	[Signal] delegate void TakeDamage (AttackResource attack, Vector2 direction);
	KinematicCollision2D collInfo = null;

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
	[Master]
	public Vector2 NextMovement {
		get { return nextMovement; }
		set {
			nextMovement = value;
			if (Network.IsConnectionStarted)
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
		if (!Network.IsConnectionStarted || IsNetworkMaster ()) { //Master Code
			// Movement in case of impact
			if (isImpact) {
				impactTime -= delta;
				collInfo = MoveAndCollide (impactDirection * delta);
				if (impactTime <= 0) {
					impactTime = 0;
					isImpact = false;
				}
			} else if (NextMovement != Vector2.Zero) { // Normal Movement
				collInfo = MoveAndCollide (NextMovement * WALK_SPEED * delta);
				NextMovement = new Vector2 (0, 0);
			}
			if (collInfo != null)
				EmitSignal ("body_collision", collInfo);
			if (Network.IsConnectionStarted)
				RsetUnreliable ("PuppetPosition", Position);
		} else { //Puppet Code
			if (PuppetPosition != Vector2.Zero)
				Position = PuppetPosition;
		}

	}

	public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		GetChild ((int) saveObject["ControlPosition"]).Call ("LoadData", saveObject);
	}

}
