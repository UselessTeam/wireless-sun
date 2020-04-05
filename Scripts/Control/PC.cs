using System;
using Godot;

public class PC : _Control {
    [Export] public float FLICKER_TIME = 3;

    bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    public new bool CanMove { get { return !isAttacking && MyBody.CanMove; } }

    public override void _Ready () {
        MyBody.Connect ("damage_taken", this, "_OnDamageTaken");
    }

    string[] ActionList = {
        "action_1" // Attack
    };

    public override void _Input (InputEvent _event) {
        if (IsMaster) {
            foreach (var act in ActionList) {
                if (_event.IsActionPressed (act)) {
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
    public void _Action (string _event) {
        if (CanMove && _event == ActionList[0]) { // Action : Attack
            GetNode<PlayerAttack> ("Attack")._StartAttack ();
            isAttacking = true;
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
        }
    }

    public override void SaveIn (Godot.Collections.Dictionary<string, object> saveObject) {
        saveObject["HP"] = GetNode<Health> ("Health").HP;
        saveObject["Damage"] = GetNode<_Attack> ("Attack").DAMAGE;
    }

    public override void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
        GetNode<Health> ("Health").HP = (float) saveObject["HP"];
        GetNode<_Attack> ("Attack").DAMAGE = (float) saveObject["Damage"];
    }

    public override void _OnDamageTaken (float damage) {
        MyBody.StartFlicker (FLICKER_TIME);
    }

    public new void _OnDied () {
        if (IsMaster) {
            Gameplay.Instance._OnPlayerDied ();
        } else {
            base._OnDied ();
        }
    }

    _Interactable currentInteraction;
    Sprite interactionSprite { get { return GetNode<Sprite> ("InteractionSprite"); } }

    public void _InteractEntered (_Interactable interaction) {
        currentInteraction = interaction;
        interactionSprite.Show ();
    }

    public void _InteractExited (_Interactable interaction) {
        if (interaction == currentInteraction)
            currentInteraction = null;
        interactionSprite.Hide ();
    }

}