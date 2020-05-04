using System;
using Godot;

public class HealthTween : Tween {
    Node2D currentNode;
    Timer flickerTimer = new Timer ();

    [Export] float FLICKER_PERIOD = 0.1f;

    public override void _Ready () {
        flickerTimer.Connect ("timeout", this, nameof (StopFlicker));
        AddChild (flickerTimer);
    }

    public void StartFlicker (Node2D flickerNode, float time) {
        InterpolateProperty (flickerNode, "modulate:a", 1, 0.2, FLICKER_PERIOD, TransitionType.Linear);
        InterpolateProperty (flickerNode, "modulate:a", 0.2, 1, FLICKER_PERIOD, TransitionType.Linear, EaseType.InOut, FLICKER_PERIOD);
        flickerTimer.Start (time);
        Repeat = true;
        Start ();
        currentNode = flickerNode;
    }
    public void StopFlicker () {
        RemoveAll ();
        currentNode.Modulate += new Color (0, 0, 0, 1);
    }
}