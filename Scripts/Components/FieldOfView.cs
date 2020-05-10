using System;
using Godot;

public class FieldOfView : Area2D {

    public bool IsPlayerDetected () {
        return GetOverlappingBodies ().Count > 0;
    }

    public Node2D GetClosestPlayer () {
        Node2D closestPlayer = null;
        float maxDist = float.PositiveInfinity;
        foreach (Node2D piece in GetOverlappingBodies ()) {
            float distance = (piece.GlobalPosition - GetParent<ControlComponent> ().MyPiece.GlobalPosition).LengthSquared ();
            if (distance < maxDist) {
                closestPlayer = piece;
                maxDist = distance;
            }
        }
        return closestPlayer;
    }
}