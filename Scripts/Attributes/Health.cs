using System;
using Godot;

public class Health : Node2D {
    // Class that manages HP, HP recovery, Death, ect ...
    [Export][Puppet] private float _HP = 50;

    [Signal] delegate void hp_changed (float HP);
    [Signal] delegate void died ();

    public override void _Ready () {
        Connect ("died", MyUser, "_OnDied");
        MyUser.MyBody.Connect ("damage_taken", this, "_OnDamageTaken");
    }

    public _Control MyUser { get { return GetParent<_Control> (); } }

    public float HP {
        get { return _HP; }
        set {
            _HP = value;
            EmitSignal ("hp_changed", _HP);
            if (MyUser.IsTrueMaster)
                Rset ("_HP", _HP);
            if (_HP <= 0 && MyUser.IsMaster) {
                if (Network.IsConnectionStarted)
                    Rpc ("Die");
                Die ();
            }
        }
    }

    [Master]
    public float DAMAGE {
        get { return DAMAGE; }
        set {
            DAMAGE = value;
            if (GetParent<_Control> ().IsTrueMaster)
                Rset ("DAMAGE", DAMAGE);
        }
    }

    public void _OnDamageTaken (float damage) {
        HP -= damage;
    }

    [Puppet]
    public void Die () {
        EmitSignal ("died");
    }
}