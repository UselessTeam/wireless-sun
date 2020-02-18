using System;
using Godot;

public abstract class _EnemiControl : Node2D {
    public float WALK_SPEED;
    public float DAMAGE;

    public bool IsCanMove () {
        return GetParent<Body> ().IsCanMove ();
    }

}