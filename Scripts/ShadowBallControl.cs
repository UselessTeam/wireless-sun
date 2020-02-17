using System;
using Godot;

public class ShadowBallControl : Node2D {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public bool IsCanMove () {
        return GetParent<Body> ().IsCanMove ();
    }

    public override void _PhysicsProcess (float delta) {
        Vector2 direction = new Vector2 (0, 0);
        GetParent<Body> ().Walk (direction, delta);
    }
}