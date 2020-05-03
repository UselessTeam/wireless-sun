using System;
using Godot;

public class HealthComponent : Node2D {
    // Class that manages HP, HP recovery, Death, ect ...
    [Export][Puppet] public float MAX_HP = 100;
    [Export][Puppet] public readonly float FLICKER_TIME = 0.2f;
    [Export] public ArmorResource armor = null;

    private float _HP;
    public bool isFlicker = false;
    private float flickerTimeLeft = 0;

    [Signal] public delegate void HpChanged (float HP);
    [Signal] public delegate void Died ();

    public override void _Ready () {
        _HP = MAX_HP;
        Connect (nameof (Died), MyUser, nameof (ControlComponent._OnDied));
        if (MyUser == null)
            GD.PrintErr ("Error in Node \"" + GetParent<KinematicPiece> ().Name + "\" : Health component requires a Control component");
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

    public void TakeDamage (AttackResource attackData, Vector2 direction) {
        if (isFlicker)
            return;
        HP -= (armor == null) ? attackData.Damage : armor.ApplyDamage (attackData.Types, attackData.Damage);
        isFlicker = true;
        float knockbackTime = attackData.Knockback / 1000.0f;
        flickerTimeLeft = FLICKER_TIME + knockbackTime;
        if (MyUser.MyMovement != null)
            MyUser.MyMovement.StartImpact (direction, knockbackTime);
        GetNode<HealthTween> ("Tween").StartFlicker (GetNode<Node2D> ("../Display"));

    }

    public override void _Process (float delta) {
        if (isFlicker) {
            flickerTimeLeft -= delta;
            if (flickerTimeLeft < 0) {
                isFlicker = false;
                flickerTimeLeft = 0;
                GetNode<HealthTween> ("Tween").StopFlicker ();
            }
        }
    }

    [Puppet]
    public void Die () {
        EmitSignal (nameof (Died));
    }
}