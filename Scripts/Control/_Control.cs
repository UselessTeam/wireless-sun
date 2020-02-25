using System;
using Godot;

public abstract class _Control : Node2D {
    [Export] public float FLICKER_TIME = 3;

    public bool CanMove { get { return GetParent<Body> ().CanMove; } }
    public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
    public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

    public Body MyBody { get { return GetParent<Body> (); } }

    public void _OnDied () {

    }

    public void _OnDamageTaken (float damage) {
        MyBody.StartFlicker (FLICKER_TIME);
    }

    // [Master] void CheckDeath

}