using System;
using Godot;

public class Body : KinematicBody2D {
    [Export] public float HP = 50;
    [Export] public float WALK_SPEED = 100;

    public bool isImpact = false;
    private float impactTime = 0;
    private Vector2 impactDirection;

    [Puppet] private Vector2 facingDirection = new Vector2 (1, 0);
    [Master] public Vector2 FacingDirection {
        get { return facingDirection; }
        set {
            if (facingDirection == Vector2.Zero)
                return;
            facingDirection = value.Normalized ();
            if (Network.isConnectionStarted)
                RsetUnreliable ("facingDirection", facingDirection);
        }
    }

    [Puppet] private Vector2 nextMovement = new Vector2 (0, 0);
    [Master] public Vector2 NextMovement {
        get { return nextMovement; }
        set {
            nextMovement = value;
            if (Network.isConnectionStarted)
                RsetUnreliable ("nextMovement", nextMovement);
        }
    }

    [Puppet] Vector2 PuppetPosition = new Vector2 (0, 0);

    public bool CanMove { get { return !isImpact; } }

    // Call whenever the body is hit for some time
    public void StartImpact (Vector2 direction, float time, float damage = 0) {
        HP -= damage;
        isImpact = true;
        impactTime = time;
        impactDirection = direction;
    }

    public override void _PhysicsProcess (float delta) {
        if (!Network.isConnectionStarted || IsNetworkMaster ()) { //Master Code
            // Movement in case of impact
            if (isImpact) {
                impactTime -= delta;
                MoveAndCollide (impactDirection * delta);
                if (impactTime <= 0) {
                    impactTime = 0;
                    isImpact = false;
                }
            } else if (NextMovement != Vector2.Zero) { // Normal Movement
                MoveAndCollide (NextMovement * WALK_SPEED * delta);
                NextMovement = new Vector2 (0, 0);
            }
            if (Network.isConnectionStarted)
                RsetUnreliable ("PuppetPosition", Position);
        } else { //Puppet Code
            if (PuppetPosition != Vector2.Zero)
                Position = PuppetPosition;
        }

    }
}