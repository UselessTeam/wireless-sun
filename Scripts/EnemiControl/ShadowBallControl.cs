using System;
using Godot;

public class ShadowBallControl : _EnemiControl {
    Body myBody;

    public override void _Ready () {
        WALK_SPEED = 30;
        DAMAGE = 10;

        myBody = GetParent<Body> ();
    }

    public override void _PhysicsProcess (float delta) {
        Vector2 direction = new Vector2 (0, 0);

        direction = (Beach.PlayerBody.Position - myBody.Position).Normalized ();
        GetParent<Body> ().Walk (direction * WALK_SPEED, delta);
    }
}