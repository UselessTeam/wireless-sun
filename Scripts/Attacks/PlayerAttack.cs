using System;
using Godot;

public class PlayerAttack : Attack {

    public float DISTANCE_TO_PLAYER = 15;

    public AnimatedSprite mySprite;

    public override void _Ready () {
        DAMAGE = 10;

        mySprite = GetNode<AnimatedSprite> ("Sprite");
        base._Ready ();
    }

    public void _OnAttackFinished () {
        mySprite.Hide ();
        mySprite.Stop ();
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
        Vector2 direction = myUserBody.facingDirection;
        Position = DISTANCE_TO_PLAYER * direction;
        if (direction.x == 0 || direction.y == 0) {
            Rotation = direction.Angle ();
            mySprite.Play ("straight");
        } else {
            // Rotation = OrientedAngle (direction, new Vector2 (1, -1));
            mySprite.Play ("diagonal");
        }
        mySprite.Show ();

        // Play Attack's SFX
        GetNode<AudioStreamPlayer2D> ("SFX").Play (0);
    }

}