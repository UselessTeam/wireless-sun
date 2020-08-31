using System;
using Godot;

public class PlayerAttack_Block : PlayerAttack_Base {
    public override void _Ready () {
        base._Ready ();
        XpStatOnTouch = "block";
    }

    public void LaunchBlock (AttackResource attackData) {
        this.attackData = attackData;
        // Enable the attack's collisionBox
        Scale = new Vector2 (attackData.Range, attackData.Range);
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = false;

        // Put the attack sprite at the right position and show it
        PositionSelf ();
        MyAttackSprite.Show ();

        // Play Attack's SFX
        GetNode<AudioStreamPlayer2D> ("SFX").Play (0);
    }

    public override void _Input (InputEvent _event) {
        if (_event is InputEventMouseMotion)
            PositionSelf ();
    }

    public void StopBlock () {
        HideAndDisable ();
    }
}