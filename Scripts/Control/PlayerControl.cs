using System;
using System.Collections.Generic;
using Godot;

public class PlayerControl : ControlComponent {
    [Export] public float FLICKER_TIME = 3;

    bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }
    public new bool CanMove { get { return !isAttacking && MyBody.CanMove; } }

    public override void _Ready () {
        base._Ready ();
    }

    [Signal] public delegate void StartCooldown (string action, float time);
    Godot.Collections.Dictionary<ActionList, float> cooldown = new Godot.Collections.Dictionary<ActionList, float> () { { ActionList.left_action, 0 }, { ActionList.right_action, 0 },
    };
    Godot.Collections.Dictionary<ActionList, string> actionToPanelName = new Godot.Collections.Dictionary<ActionList, string> () { { ActionList.left_action, "LeftHand" }, { ActionList.right_action, "RightHand" },
    };

    public enum ActionList {
        left_action,
        right_action
    }

    public override void _Input (InputEvent _event) {
        if (IsMaster) {
            foreach (ActionList act in Enum.GetValues (typeof (ActionList))) {
                if (_event.IsActionPressed (act.ToString ())) {
                    if (cooldown[act] > 0)
                        return;
                    if (Network.IsConnectionStarted) {
                        // If connected to a server, the event will be sent to all puppets
                        this.Rpc ("_Action", act);
                        _Action (act);
                    } else
                        // If not it will just be performed normally
                        _Action (act);
                    return;
                }
            }
            if (_event is InputEventMouseMotion eventMouseMotion) {
                MyBody.FacingDirection = GetGlobalMousePosition () - MyBody.GlobalPosition;
            }
        }
    }

    [Puppet]
    public void _Action (ActionList _action) {
        if (CanMove && _action.ToString ().Contains ("_action")) {
            var weaponData = GameRoot.inventory.equipement.GetAction (_action == ActionList.left_action);
            // if (weaponData.Action == ActionType.Attack) {
            GetNode<PlayerAttack> (weaponData.Action.ToString ())._StartAttack (weaponData.Action, weaponData.AttackData);
            isAttacking = true;
            // } else if (weaponData.Action == ActionType.Block) {
            //     GD.Print ("Block!");
            // }
            EmitSignal (nameof (StartCooldown), (_action == ActionList.left_action) ? "LeftHand" : "RightHand", weaponData.Cooldown);
            // cooldownedActions.Add (_action);
            cooldown[_action] = weaponData.Cooldown;
        }
    }

    public override void _Process (float delta) {
        if (IsMaster && CanMove) { // Master Code
            Vector2 inputMovement = new Vector2 (0, 0);
            if (Input.IsActionPressed ("ui_up"))
                inputMovement.y = -1;
            if (Input.IsActionPressed ("ui_down"))
                inputMovement.y = 1;
            if (Input.IsActionPressed ("ui_left"))
                inputMovement.x = -1;
            if (Input.IsActionPressed ("ui_right"))
                inputMovement.x = 1;
            if (inputMovement != Vector2.Zero) {
                inputMovement = inputMovement.Normalized ();
                MyBody.NextMovement = inputMovement;
                inputMovement = new Vector2 (0, 0);
            }
            if (Input.IsActionPressed ("interact") && currentInteraction != null)
                currentInteraction.Interact ();
            foreach (var act in cooldown) {
                if (act.Value < 0)
                    continue;
                cooldown[act.Key] -= delta;
            }
        }
    }

    public new Godot.Collections.Dictionary<string, object> MakeSave () {
        var saveObject = base.MakeSave ();
        saveObject["HP"] = GetNode<HealthComponent> ("../Health").HP;
        if (IsMaster)
            saveObject["MyPlayer"] = true;
        return saveObject;
    }

    public new void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
        base.LoadData (saveObject);
        GetNode<HealthComponent> ("../Health").HP = Convert.ToSingle (saveObject["HP"]);
        if (saveObject.ContainsKey ("MyPlayer"))
            MyBody.Name = "MyPlayer";
    }

    public new void _OnDied () {
        if (IsMaster) {
            Gameplay.Instance._OnPlayerDied ();
        } else {
            base._OnDied ();
        }
    }

    InteractionComponent currentInteraction;
    Sprite interactionSprite { get { return GetNode<Sprite> ("InteractionSprite"); } }

    public void _InteractEntered (InteractionComponent interaction) {
        currentInteraction = interaction;
        interactionSprite.Show ();
    }

    public void _InteractExited (InteractionComponent interaction) {
        if (interaction == currentInteraction)
            currentInteraction = null;
        interactionSprite.Hide ();
    }

}