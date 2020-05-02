using System;
using Godot;

public class HealthTween : Tween {
    Node2D currentNode;

    [Export] float FLICKER_PERIOD = 0.1f;
    public override void _Ready () {}
    public void StartFlicker (Node2D flickerNode) {
        InterpolateProperty (flickerNode, "modulate:a", 1, 0.2, FLICKER_PERIOD, TransitionType.Linear);
        InterpolateProperty (flickerNode, "modulate:a", 0.2, 1, FLICKER_PERIOD, TransitionType.Linear, EaseType.InOut, FLICKER_PERIOD);
        Repeat = true;
        Start ();
        currentNode = flickerNode;
    }
    public void StopFlicker () {
        RemoveAll ();
        currentNode.Modulate += new Color (0, 0, 0, 1);
    }
}