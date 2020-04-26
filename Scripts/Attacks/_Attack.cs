using System;
using Godot;

public class _Attack : Node2D {
	[Export] public AttackTemplate attackData = null;

	[Export] public float baseDamage = 50;

	public Area2D MyArea { get { return GetNode<Area2D> ("Hitbox"); } }

	public _Control MyUser { get { return GetParent<_Control> (); } }
	public Body MyUserBody { get { return MyUser.MyBody; } }

	public override void _Ready () {}

	public override void _Process (float delta) {
		foreach (Body attackedBody in MyArea.GetOverlappingBodies ()) {
			attackedBody.StartImpact ((attackedBody.GlobalPosition - MyUserBody.GlobalPosition).Normalized () * 800, 0.1f, GetDamage ());
		}
	}

	public virtual float GetDamage () { return baseDamage * ((attackData == null) ? 1 : attackData.Damage);; }
}