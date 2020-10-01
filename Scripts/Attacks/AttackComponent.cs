using System;
using Godot;

public class AttackComponent : Node2D {
	[Export] public AttackResource attackData = new AttackResource (50, 50, 1, 1);

	public PlayerControl MyPlayer = null; // Reference to the player who owns the attack (null if the owner is not a player) 

	public Area2D MyArea { get { return GetNode<Area2D> ("Hitbox"); } }

	public ControlComponent MyUser { get { return GetParent<ControlComponent> (); } }
	public MovementComponent MyUserMovement { get { return MyUser.MyMovement; } }

	public override void _Process (float delta) {
		foreach (Node2D attackedPiece in MyArea.GetOverlappingBodies ()) {
			var health = attackedPiece.GetNodeOrNull<HealthComponent> ("Health");
			var damage = 0f;
			if (health != null)
				damage = health.TakeDamage (attackData, (attackedPiece.GlobalPosition - MyUserMovement.GlobalPosition).Normalized ());
			if (MyPlayer != null) { MyPlayer.GatherXP (attackData, damage, attackedPiece); }
		}
	}

	public virtual float GetDamage () { return attackData.Damage; }
}
