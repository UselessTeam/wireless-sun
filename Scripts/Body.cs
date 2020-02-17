using System;
using Godot;

public class Body : KinematicBody2D {
    public const float WALK_SPEED = 100;

    public bool isImpact = false;
    private float impactTime = 0;
    private float impactDirection = 0;

    public Vector2 facingDirection = new Vector2 (1, 0);

    public bool IsCanMove () {
        return !isImpact;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready () { }

    public void Walk (Vector2 direction, float delta) {
        MoveAndCollide (direction * delta * WALK_SPEED);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }
}