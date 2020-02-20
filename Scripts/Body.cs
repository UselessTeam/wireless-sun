using System;
using Godot;

public class Body : KinematicBody2D {
    [Export] public int HP = 50;
    [Export] public float WALK_SPEED = 100;

    public bool isImpact = false;
    private float impactTime = 0;
    private Vector2 impactDirection;

    [RemoteSync] private Vector2 facingDirection = new Vector2 (1, 0);
    public Vector2 FacingDirection {
        get { return facingDirection; } set {
            if (facingDirection == Vector2.Zero)
                return;
            facingDirection = value.Normalized ();
            if (Network.isConnectionStarted)
                RsetUnreliable ("facingDirection", facingDirection);
        }
    }

    [RemoteSync] private Vector2 nextMovement = new Vector2 (0, 0);
    public Vector2 NextMovement {
        get { return nextMovement; }
        set {
            nextMovement = value;
            if (Network.isConnectionStarted)
                RsetUnreliable ("nextMovement", nextMovement);
            // if (value != Vector2.Zero) facingDirection = value.Normalized ();
        }
    }

    [Puppet] Vector2 PuppetPosition = new Vector2 (0, 0);

    public bool CanMove { get { return !isImpact; } }

    public override void _Ready () { }

    public void Walk (Vector2 direction, float delta) {
        MoveAndCollide (direction * delta);
    }

    // Call to start an impact
    public void StartImpact (Vector2 direction, float time, float damage = 0) {
        isImpact = true;
        impactTime = time;
        impactDirection = direction;
    }

    public override void _PhysicsProcess (float delta) {
        if (!Network.isConnectionStarted || IsNetworkMaster ()) { //Master Code
            // Impact movement
            if (isImpact) {
                impactTime -= delta;
                MoveAndCollide (impactDirection * delta);
                if (impactTime <= 0) {
                    impactTime = 0;
                    isImpact = false;
                }
            }
            // NormalMovement
            else {
                MoveAndCollide (nextMovement * WALK_SPEED * delta);
                nextMovement = new Vector2 (0, 0);
            }

            if (Network.isConnectionStarted)
                RsetUnreliable ("PuppetPosition", Position);
        } else { //Puppet Code
            if (PuppetPosition != Vector2.Zero)
                Position = PuppetPosition;
        }

    }
}