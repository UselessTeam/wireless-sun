using System;
using Godot;

public class PlayerAttack_Attack : PlayerAttack_Base {

    public override void _Ready () {
        base._Ready ();
        MyAttackSprite.Connect ("animation_finished", this, nameof (_OnAttackFinished));
    }

    public void ChargeAttack (AttackResource attackData) {
        //Insert animation and stuff
    }

    public void LaunchAttack (AttackResource attackData) {
        this.attackData = attackData;
        LaunchAttack ();
    }
    public void LaunchAttack () {
        // Enable the attack's collisionBox
        Scale = new Vector2 (attackData.Range, attackData.Range);
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = false;

        // Put the attack sprite at the right position and show it
        PositionSelf ();
        if (Rotation % Math.PI / 2 < LimitAngle && (-Rotation) % Math.PI / 2 < LimitAngle) {
            MyAttackSprite.Play ("straight");
        } else {
            Rotation += (float) Math.PI / 4;
            MyAttackSprite.Play ("diagonal");
        }
        MyAttackSprite.Show ();

        // Play Attack's SFX
        GetNode<AudioStreamPlayer2D> ("SFX").Play (0);
    }

    //
    // The attack ends when the attack animation is finished
    // The player is then allowed to start a new attack
    public void _OnAttackFinished () {
        HideAndDisable ();
    }

}