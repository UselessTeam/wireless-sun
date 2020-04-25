using System;
using Godot;

public enum AttackType {
	Hand,
	Sword,
	Axe,
	Hoe,
	Fire
}

public class _Attack : Node2D {
	[Export] public Item.WeaponData weaponData;

	[Export] public float damage = 10;
	public AttackType TYPE = AttackType.Hand;

	public Area2D MyArea { get { return GetNode<Area2D> ("Hitbox"); } }

	public _Control MyUser { get { return GetParent<_Control> (); } }
	public Body MyUserBody { get { return MyUser.MyBody; } }

	public override void _Ready () {}

	public override void _Process (float delta) {
		foreach (Body attackedBody in MyArea.GetOverlappingBodies ()) {
			attackedBody.StartImpact ((attackedBody.GlobalPosition - MyUserBody.GlobalPosition).Normalized () * 800, 0.1f, GetDamage ());
		}
	}

	public virtual float GetDamage () { return damage; }
}