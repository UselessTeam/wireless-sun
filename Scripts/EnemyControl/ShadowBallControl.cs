using System;
using Godot;

public class ShadowBallControl : _EnemyControl {
    Body myBody {
        get { return GetParent<Body> (); }
    }
    FieldOfView myFOV {
        get { return GetNode<FieldOfView> ("../FieldOfView"); }
    }

    public override void _Ready () { }

    public bool CanSeePlayer {
        get { return myFOV.IsPlayerDetected (); }
    }

    public override void _PhysicsProcess (float delta) {
        var direction = new Vector2 (0, 0);

        if (CanSeePlayer && CanMove) {
            var playerBody = myFOV.GetClosestPlayer ();
            direction = (playerBody.Position - myBody.Position).Normalized ();
            myBody.NextMovement = direction;
        }
    }
}