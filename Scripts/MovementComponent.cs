using System;
using Godot;

public class MovementComponent : KinematicBody2D {
    [Export] public float IMPACT_FACTOR = 800;
    [Export] public float WALK_SPEED = 100;

    public static Vector2 IsometricMultiplier = new Vector2 (1.5f, 1);

}