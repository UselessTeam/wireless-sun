using System;
using Godot;

public abstract class Attack : Node2D {

	public float DAMAGE;

	public Area2D myArea;

	public Body myUserBody;

	public override void _Ready () {
		myArea = GetNode<Area2D> ("Hitbox");
		myUserBody = GetParent<Body> ();

	}

	public override void _Process (float delta) {
		foreach (Body attackedBody in myArea.GetOverlappingBodies ()) {
			attackedBody.StartImpact ((attackedBody.Position - myUserBody.Position).Normalized () * 800, 0.1f);
		}
	}
}