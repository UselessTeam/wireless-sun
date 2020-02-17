using System;
using Godot;

public class Beach : Node2D {
    static public PC Player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {
        Player = GetNode<PC> ("PlayerBody/PlayerControl");
    }

}