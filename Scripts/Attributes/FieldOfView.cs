using System;
using Godot;

public class FieldOfView : Area2D {

	public bool IsPlayerDetected () {
		return GetOverlappingBodies ().Count > 0;
	}

	public Body GetClosestPlayer () {
		Body closestPlayer = null;
		float maxDist = float.PositiveInfinity;
		foreach (PhysicsBody2D body in GetOverlappingBodies ()) {
			float distance = (body.GlobalPosition - GetParent<_Control> ().MyBody.GlobalPosition).LengthSquared ();
			if (distance < maxDist) {
				closestPlayer = (Body) body;
				maxDist = distance;
			}
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
