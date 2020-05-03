using System;
using Godot;

public abstract class PlayerAttack_Base : AttackComponent {

    [Export] public float base_distance_to_player = 15;
    protected readonly double LimitAngle = Math.PI / 8; //Limite a partir de laquelle l'attaque est horizontale

    public AnimatedSprite MyAttackSprite {
        get { return GetNode<AnimatedSprite> ("Sprite"); }
    }

    public override void _Process (float delta) { base._Process (delta); }

    public void PositionSelf () {
        Vector2 direction = MyUserMovement.FacingDirection;
        Position = base_distance_to_player * attackData.Range * direction;
        Rotation = direction.Angle ();
    }

    public void HideAndDisable () {
        MyAttackSprite.Hide ();
        MyAttackSprite.Stop ();
        GetParent<PlayerControl> ()._AttackFinished ();
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = true;
    }
}