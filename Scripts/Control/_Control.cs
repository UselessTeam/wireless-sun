using System;
using Godot;

public abstract class _Control : Node2D {
    public override void _Ready () {
        AddToGroup ("SaveNodes");
        MyBody.Connect ("damage_taken", this, "_OnDamageTaken");
    }

    public bool CanMove { get { return GetParent<Body> ().CanMove; } }
    public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
    public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

    public Body MyBody { get { return GetParent<Body> (); } }

    public virtual void _OnDamageTaken (float damage) {}

    public void _OnDied () {
        MyBody.QueueFree ();
    }

    // Save and load data of the object in a file
    public Godot.Collections.Dictionary<string, object> MakeSave () {
        var saveObject = new Godot.Collections.Dictionary<string, object> () {
                {
                "Filename",
                MyBody.Filename
                }, { "Name", MyBody.Name }, {
                "Parent",
                MyBody.GetParent ().GetPath ()
                }, { "ControlPosition", GetPositionInParent () }, {
                "PositionX",
                MyBody.Position.x
                }, { "PositionY", MyBody.Position.y }
            };
        return saveObject;
    }

    public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
        MyBody.Position = new Vector2 (Convert.ToSingle (saveObject["PositionX"]), Convert.ToSingle (saveObject["PositionY"]));
    }
}