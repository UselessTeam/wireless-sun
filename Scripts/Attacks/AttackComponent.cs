using System;
using Godot;

public class AttackComponent : Node2D {
    [Export] public AttackResource attackData = new AttackResource (50, 50, 1, 1);

    public Area2D MyArea { get { return GetNode<Area2D> ("Hitbox"); } }

    public ControlComponent MyUser { get { return GetParent<ControlComponent> (); } }
    public MovementComponent MyUserMovement { get { return MyUser.MyMovement; } }

    public override void _Process (float delta) {
        foreach (Node2D attackedPiece in MyArea.GetOverlappingBodies ()) {
            var health = attackedPiece.GetNodeOrNull<HealthComponent> ("Health");
            if (health != null)
                health.TakeDamage (attackData, (attackedPiece.GlobalPosition - MyUserMovement.GlobalPosition).Normalized ());
        }
    }

    public virtual float GetDamage () { return attackData.Damage; }
}