using System;
using Godot;

public abstract class _EnemyControl : Node2D {
    [Export] public float DAMAGE = 10;

    public bool CanMove { get { return GetParent<Body> ().CanMove; } }
    protected bool isMaster { get { return !Network.isConnectionStarted || IsNetworkMaster (); } }

}