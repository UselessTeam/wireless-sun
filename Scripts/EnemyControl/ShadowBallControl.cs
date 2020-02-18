using System;
using Godot;

public class ShadowBallControl : _EnemyControl {
    Body myBody;
    FieldOfView myFOV;

    public override void _Ready () {
        WALK_SPEED = 50;
        DAMAGE = 10;
        DETECTION_AREA = 200;

        myBody = GetParent<Body> ();
        myFOV = GetNode<FieldOfView> ("../FieldOfView");
    }

    public bool CanSeePlayer () {
        return myFOV.IsPlayerDetected ();
    }

    public override void _PhysicsProcess (float delta) {
        var direction = new Vector2 (0, 0);

        if (CanSeePlayer () && myBody.CanMove ()) {
            var playerBody = myFOV.GetClosestPlayer ();
            direction = (playerBody.Position - myBody.Position).Normalized ();
            myBody.MoveAndCollide (direction * WALK_SPEED * delta);
        }
    }
}