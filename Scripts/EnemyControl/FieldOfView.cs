using System;
using Godot;

public class FieldOfView : Area2D {

    public bool IsPlayerDetected () {
        return GetOverlappingBodies ().Count > 0;
    }

    public Body GetClosestPlayer () {
        Body closestPlayer = null;
        float distance = float.PositiveInfinity;
        foreach (PhysicsBody2D body in GetOverlappingBodies ()) {
            if ((body.Position - GetParent<Body> ().Position).LengthSquared () < distance)
                closestPlayer = (Body) body;
        }
        return closestPlayer;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }
}