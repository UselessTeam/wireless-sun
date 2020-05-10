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
        MyAttackSprite.Play ("normal");
        MyAttackSprite.Show ();

        // Play Attack's SFX
        GetNode<AudioStreamPlayer2D> ("SFX").Play (0);
    }

    int rotationStep = 0;
    Vector2[] rotationDirections = new Vector2[] { new Vector2 (1, 0), new Vector2 (1, 1).Normalized (), new Vector2 (0, 1), new Vector2 (-1, 1).Normalized () };
    public void LaunchCircularAttack (AttackResource attackData) {
        this.attackData = attackData;

        // Enable the attack's collisionBox
        Scale = new Vector2 (attackData.Range, attackData.Range);
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = false;

        // Change the position of the attack at every frame, and pake i
        PositionSelf (rotationDirections[0]);
        MyAttackSprite.Connect ("frame_changed", this, nameof (NextRotationStep));
        MyAttackSprite.Play ("circular");
        MyAttackSprite.Show ();

        // Play Attack's SFX
        GetNode<AudioStreamPlayer2D> ("SFX").Play (0);
    }
    public void NextRotationStep () {
        rotationStep += 1;
        PositionSelf (rotationDirections[rotationStep % 4] * ((rotationStep >= 4) ? -1 : 1));
    }

    //
    // The attack ends when the attack animation is finished
    // The player is then allowed to start a new attack
    public void _OnAttackFinished () {
        HideAndDisable ();
        if (rotationStep > 0) {
            rotationStep = 0;
            MyAttackSprite.Disconnect ("frame_changed", this, nameof (NextRotationStep));
        }
    }

}