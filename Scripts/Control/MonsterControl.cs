using System;
using Godot;

public class MonsterControl : ControlComponent {
    [Export] public float FLICKER_TIME = 3;

    public ControlComponent MyControl {
        get { return MyPiece.GetNode<ControlComponent> ("Control"); }
    }

    public FieldOfView MyFOV {
        get { return GetNode<FieldOfView> ("FieldOfView"); }
    }

    public bool CanSeePlayer {
        get { return MyFOV.IsPlayerDetected (); }
    }

    public new Godot.Collections.Dictionary<string, object> MakeSave () {
        var saveObject = base.MakeSave ();
        saveObject["HP"] = GetNode<HealthComponent> ("../Health").HP;
        return saveObject;
    }

    public new void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
        base.LoadData (saveObject);
        GetNode<HealthComponent> ("../Health").HP = Convert.ToSingle (saveObject["HP"]);
    }
}