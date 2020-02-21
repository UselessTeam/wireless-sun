using System;
using Godot;

public class PlayerAttack : Attack {

    [Export] public float DISTANCE_TO_PLAYER = 15;

    double LimitAngle = Math.PI / 8; //Limite a partir de laquelle l'attaque est horizontale

    public AnimatedSprite MySprite {
        get { return GetNode<AnimatedSprite> ("Sprite"); }
    }

    public override void _Ready () {
        base._Ready ();
    }

    public void _OnAttackFinished () {
        MySprite.Hide ();
        MySprite.Stop ();
        GetNode<PC> ("../PlayerControl").isAttacking = false;

        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = true;
    }

    public float OrientedAngle (Vector2 U, Vector2 V) {
        float alpha = U.Cross (V);
        return U.AngleTo (V) * alpha / Math.Abs (alpha);
    }

    public void _StartAttack () {
        // Enable the attack's collisionBox
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = false;

        // Put the attack sprite at the right position and show it
        Vector2 direction = MyUserBody.FacingDirection;
        Position = DISTANCE_TO_PLAYER * direction;
        Rotation = direction.Angle ();
        if (Rotation % Math.PI / 2 < LimitAngle && (-Rotation) % Math.PI / 2 < LimitAngle) {
            MySprite.Play ("straight");
        } else {
            // Rotation = OrientedAngle (direction, new Vector2 (1, -1));
            Rotation += (float) Math.PI / 4;
            MySprite.Play ("diagonal");
        }
        MySprite.Show ();

        // Play Attack's SFX
        GetNode<AudioStreamPlayer2D> ("SFX").Play (0);
    }

}