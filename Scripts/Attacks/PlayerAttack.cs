using System;
using Godot;

public class PlayerAttack : AttackComponent {

    [Export] public float base_distance_to_player = 15;
    readonly double LimitAngle = Math.PI / 8; //Limite a partir de laquelle l'attaque est horizontale

    ActionType action;

    public AnimatedSprite MySprite {
        get { return GetNode<AnimatedSprite> ("Sprite"); }
    }

    public override void _Ready () {
        base._Ready ();
    }

    public override void _Process (float delta) {
        base._Process (delta);
    }

    public void _StartAttack (ActionType action, AttackResource attackData) {
        this.attackData = attackData;
        this.action = action;
        // Enable the attack's collisionBox
        Scale = new Vector2 (attackData.Range, attackData.Range);
        GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = false;

        // Put the attack sprite at the right position and show it
        PositionSelf ();
        if (action == ActionType.Attack)
            if (Rotation % Math.PI / 2 < LimitAngle && (-Rotation) % Math.PI / 2 < LimitAngle) {
                MySprite.Play ("straight");
            } else {
                Rotation += (float) Math.PI / 4;
                MySprite.Play ("diagonal");
            }
        MySprite.Show ();

        // Play Attack's SFX
        GetNode<AudioStreamPlayer2D> ("SFX").Play (0);
    }

    public void PositionSelf () {
        Vector2 direction = MyUserBody.FacingDirection;
        Position = base_distance_to_player * attackData.Range * direction;
        Rotation = direction.Angle ();
    }

    public override void _Input (InputEvent _event) {
        if (_event is InputEventMouseMotion)
            PositionSelf ();
    }

    //
    // The attack ends when the attack animation is finished
    // The player is then allowed to start a new attack
    public void _OnAttackFinished () {
        if (action == ActionType.Attack) {
            MySprite.Hide ();
            MySprite.Stop ();
            GetParent<PlayerControl> ().IsAttacking = false;
            GetNode<CollisionShape2D> ("Hitbox/CollisionShape2D").Disabled = true;
        } else {}
    }
}