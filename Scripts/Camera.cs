using System;
using Godot;

public class Camera : Camera2D {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    static Vector2 myPosition;
    public static Vector2 MyPosition { get { return myPosition; } }

    // Called when the node enters the scene tree for the first time.
    public override void _Process (float delta) {
        myPosition = GlobalPosition;
    }

}