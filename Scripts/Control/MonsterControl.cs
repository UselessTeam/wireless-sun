using System;
using Godot;

public enum DiagonalDirection {
    front_left,
    front_right,
    back_left,
    back_right
}

public class MonsterControl : ControlComponent {
    [Export] public float FLICKER_TIME = 3;

    private DiagonalDirection currentDirection;
    public DiagonalDirection CurrentDirection {
        get { return currentDirection; }
        set {
            if (value != currentDirection) {
                currentDirection = value;
                SetAnimation ();
            }
        }
    }

    private string currentState = "idle";
    public string CurrentState {
        get { return currentState; }
        set {
            if (value != currentState) {
                currentState = value;
                SetAnimation ();
            }
        }
    }

    public FieldOfView MyFOV {
        get { return GetNode<FieldOfView> ("FieldOfView"); }
    }

    public bool CanSeePlayer {
        get { return MyFOV.IsPlayerDetected (); }
    }

    public AnimatedSprite MySprite {
        get { return MyPiece.GetNode<AnimatedSprite> ("Display"); }
    }

    protected void SetAnimation () {
        string suffix = ((int) currentDirection < 2) ? "front" : "back";
        MySprite.Play (currentState + "_" + suffix);
        MySprite.FlipH = (int) currentDirection % 2 == 0;
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