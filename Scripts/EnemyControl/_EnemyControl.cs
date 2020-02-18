using System;
using Godot;

public abstract class _EnemyControl : Node2D {
    public float WALK_SPEED;
    public float DAMAGE;
    public float DETECTION_AREA;

    public bool CanMove () {
        return GetParent<Body> ().CanMove ();
    }

}