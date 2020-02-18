using System;
using Godot;

public class Beach : Node2D {
    static public Body PlayerBody;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {
        PlayerBody = GetNode<Body> ("PlayerBody");
    }

}