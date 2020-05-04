using System;
using Godot;

public class HealthComponent : Node2D {
    // Class that manages HP, HP recovery, Death, ect ...
    [Export][Puppet] public float MAX_HP = 100;
    [Export][Puppet] public readonly float FLICKER_TIME = 0.2f;
    [Export] public ArmorResource armor = null;

    private float _HP;
    public bool isInvulnerable = false;
    private float invlunerabilityTimeLeft = 0;

    [Signal] public delegate void HpChanged (float HP);
    [Signal] public delegate void Died ();

    public override void _Ready () {
        _HP = MAX_HP;
        Connect (nameof (Died), MyUser, nameof (ControlComponent._OnDied));
        if (MyUser == null)
            GD.PrintErr ("Error in Node \"" + GetParent<Node2D> ().Name + "\" : Health component requires a Control component");
    }

    public ControlComponent MyUser { get { return GetNodeOrNull<ControlComponent> ("../Control"); } }

    public float HP {
        get { return _HP; }
        set {
            _HP = value;
            if (_HP > MAX_HP)
                _HP = MAX_HP;
            EmitSignal (nameof (HpChanged), _HP);
            if (MyUser.IsTrueMaster)
                Rset (nameof (_HP), _HP);
            if (_HP <= 0 && MyUser.IsMaster) {
                if (Network.IsConnectionStarted)
                    Rpc ("Die");
                Die ();
            }
        }
    }

    public void MakeInvulnerable (float time) {
        isInvulnerable = true;
        invlunerabilityTimeLeft = Mathf.Max (time, invlunerabilityTimeLeft);
    }

    public void TakeDamage (AttackResource attackData, Vector2 direction) {
        if (isInvulnerable)
            return;
        HP -= (armor == null) ? attackData.Damage : armor.ApplyDamage (attackData.Types, attackData.Damage);
        float knockbackTime = attackData.Knockback / 1000.0f;
        float realFlickerTime = FLICKER_TIME + knockbackTime;
        MakeInvulnerable (realFlickerTime);
        if (MyUser.MyMovement != null)
            MyUser.MyMovement.StartImpact (direction, knockbackTime);
        GetNode<HealthTween> ("Tween").StartFlicker (GetNode<Node2D> ("../Display"), realFlickerTime);

    }

    public override void _Process (float delta) {
        if (isInvulnerable) {
            invlunerabilityTimeLeft -= delta;
            if (invlunerabilityTimeLeft < 0) {
                isInvulnerable = false;
                invlunerabilityTimeLeft = 0;
            }
        }
    }

    [Puppet]
    public void Die () {
        EmitSignal (nameof (Died));
    }
}