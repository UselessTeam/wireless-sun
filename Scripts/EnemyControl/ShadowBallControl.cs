using System;
using Godot;

public class ShadowBallControl : _EnemyControl {
    Body myBody;

    public override void _Ready () {
        WALK_SPEED = 50;
        DAMAGE = 10;
        DETECTION_AREA = 200;

        myBody = GetParent<Body> ();
    }

    public bool CanSeePlayer () {
        return DETECTION_AREA * DETECTION_AREA > (Beach.PlayerBody.Position - myBody.Position).LengthSquared ();
    }

    public override void _PhysicsProcess (float delta) {
        var direction = new Vector2 (0, 0);

        if (CanSeePlayer () && myBody.CanMove ()) {
            direction = (Beach.PlayerBody.Position - myBody.Position).Normalized ();
            var collInfo = myBody.MoveAndCollide (direction * WALK_SPEED * delta);
            if (collInfo != null) {
                var collNorm = collInfo.Normal;
                var collBody = (Body) collInfo.Collider;
                if (collBody.CollisionLayer == Beach.Layer["player"])
                    collBody.Impact (-collNorm * 800, 0.2f);
            }
        }
    }
}