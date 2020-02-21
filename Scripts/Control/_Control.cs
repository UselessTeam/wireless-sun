using System;
using Godot;

public abstract class _Control : Node2D {
    public bool CanMove { get { return GetParent<Body> ().CanMove; } }
    public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
    public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

    public Body MyBody { get { return GetParent<Body> (); } }

    public void _OnDied () {

    }

    // [Master] void CheckDeath

}