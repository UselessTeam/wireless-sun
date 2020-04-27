using System;
using Godot;

public class PlayerAttack : _Attack {

    [Export] public float base_distance_to_player = 15;

    double LimitAngle = Math.PI / 8; //Limite a partir de laquelle l'attaque est horizontale

    public AnimatedSprite MySprite {
        get { return GetNode<AnimatedSprite> ("Sprite"); }
    }

    public override void _Ready () {
        base._Ready ();
    }

    public override void _Process (float delta) {
        base._Process (delta);
    }

    public float OrientedAngle (Vector2 U, Vector2 V) {
        float alpha = U.Cross (V);
        return U.AngleTo (V) * alpha / Math.Abs (alpha);
    }

    public void _StartAttack (AttackResource attackData) {
        this.attackData = attackData;
        // Enable the attack's collisionBox
        Scale = new Vector2 (attackData.Range, attackData.Range);
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = false;

        // Put the attack sprite at the right position and show it
        Vector2 direction = MyUserBody.FacingDirection;
        Position = base_distance_to_player * attackData.Range * direction;
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

    //
    // The attack ends when the attack animation is finished
    // The player is then allowed to start a new attack
    public void _OnAttackFinished () {
        MySprite.Hide ();
        MySprite.Stop ();
        GetParent<PC> ().IsAttacking = false;
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = true;
    }
}