using System;
using Godot;

public class AttackComponent : Node2D {
	[Export] public AttackResource attackData = new AttackResource (50, 50, 1, 1);

	public Area2D MyArea { get { return GetNode<Area2D> ("Hitbox"); } }

	public ControlComponent MyUser { get { return GetParent<ControlComponent> (); } }
	public Body MyUserBody { get { return MyUser.MyBody; } }

	public override void _Ready () {}

	public override void _Process (float delta) {
		foreach (Body attackedBody in MyArea.GetOverlappingBodies ()) {
			attackedBody.EmitSignal ("TakeDamage", attackData, (attackedBody.GlobalPosition - MyUserBody.GlobalPosition).Normalized ());
		}
	}

	public virtual float GetDamage () { return attackData.Damage; }
}