using System;
using Godot;

public abstract class _Control : Node2D {

    public bool CanMove { get { return GetParent<Body> ().CanMove; } }
    public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
    public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

    public Body MyBody { get { return GetParent<Body> (); } }

    public virtual void _OnDamageTaken (float damage) { }

    public void _OnDied () {
        MyBody.QueueFree ();
    }

    // Save and load data of the object in a file
    public abstract void SaveIn (Godot.Collections.Dictionary<string, object> saveObject);
    public abstract void LoadData (Godot.Collections.Dictionary<string, object> saveObject);

}