using System;
using Godot;

public class Body : KinematicBody2D {
    public bool isImpact = false;
    private float impactTime = 0;
    private Vector2 impactDirection;

    public Vector2 facingDirection = new Vector2 (1, 0);

    public bool IsCanMove () {
        return !isImpact;
    }

    public override void _Ready () { }

    public void Walk (Vector2 direction, float delta) {
        MoveAndCollide (direction * delta);
    }

    // Call to start an impact
    public void Impact (Vector2 direction, float time) {
        isImpact = true;
        impactTime = time;
        direction = impactDirection;
    }

    public override void _PhysicsProcess (float delta) {
        // Impact movement
        if (isImpact) {
            impactTime -= delta;
            MoveAndCollide (impactDirection * delta);

            if (impactTime <= 0) {
                impactTime = 0;
                isImpact = false;
            }
        }
    }
}