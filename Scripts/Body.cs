using System;
using Godot;

public class Body : KinematicBody2D {
    public int HP = 50;

    public bool isImpact = false;
    private float impactTime = 0;
    private Vector2 impactDirection;

    public Vector2 facingDirection = new Vector2 (1, 0);

    [Puppet] Vector2 PuppetPosition = new Vector2 (0, 0);

    public bool CanMove () {
        return !isImpact;
    }

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

            if (Network.isConnectionStarted)
                RsetUnreliable ("PuppetPosition", Position);
        } else { //Puppet Code
            if (PuppetPosition.LengthSquared () != 0)
                Position = PuppetPosition;
        }

    }
}