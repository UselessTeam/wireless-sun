using System;
using Godot;

public class ShadowBallControl : _Control {
    FieldOfView myFOV {
        get { return GetNode<FieldOfView> ("../FieldOfView"); }
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
}