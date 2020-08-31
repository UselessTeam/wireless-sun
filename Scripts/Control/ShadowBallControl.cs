using System;
using Godot;

public class ShadowBallControl : ControlComponent {
    [Export] public float FLICKER_TIME = 3;

    bool isRunning = false;

    FieldOfView myFOV {
        get { return GetNode<FieldOfView> ("FieldOfView"); }
    }

    public bool CanSeePlayer {
        get { return myFOV.IsPlayerDetected (); }
    }

    public AnimatedSprite MyAttackSprite {
        get { return MyPiece.GetNode<AnimatedSprite> ("Display"); }
    }

    public override void _PhysicsProcess (float delta) {
        if (IsMaster) {
            if (isRunning != (CanSeePlayer && CanMove)) {
                isRunning = !isRunning;
                MyAttackSprite.Play ((isRunning) ? "run" : "idle");
            }
            var direction = new Vector2 (0, 0);
            if (isRunning) {
                var playerPiece = myFOV.GetClosestPlayer ();
                direction = (playerPiece.GlobalPosition - MyPiece.GlobalPosition).Normalized ();
                MyMovement.NextMovement = direction;
            }
        }
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