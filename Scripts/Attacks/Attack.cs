using System;
using Godot;

public class Attack : Node2D {

	[Export] public float DAMAGE = 10;

	public Area2D MyArea { get { return GetNode<Area2D> ("Hitbox"); } }

	public Body MyUserBody { get { return GetParent<Body> (); } }

	public override void _Ready () { }

	public override void _Process (float delta) {
		foreach (Body attackedBody in MyArea.GetOverlappingBodies ()) {
			attackedBody.StartImpact ((attackedBody.Position - MyUserBody.Position).Normalized () * 800, 0.1f);
		}
	}
}