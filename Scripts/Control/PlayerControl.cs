using System;
using System.Collections.Generic;
using Godot;

public class PlayerControl : ControlComponent {
    [Export] public float FLICKER_TIME = 3;
    // [Export] public float BUFFER_TIME = 0.5f;
    [Export] public float CHARGE_TIME = 1;
    [Export] public float CHARGE_SPEED_MULTIPLIER = 0.5f;
    public AttackTemplate CHARGE_TEMPLATE = new AttackTemplate (3, 1.5f, 2, 1.5f);
    [Export] public float BLOCK_SPEED_MULTIPLIER = 0.3f;

    WeaponResource currentlyPerformed = null;
    public bool IsAttacking { get { return currentlyPerformed != null; } }
    float chargedTime;
    bool isCharging = false;
    public bool IsCharging {
        get { return isCharging; }
        set {
            isCharging = value;
            if (isCharging)
                EmitSignal (nameof (StartCharge), CHARGE_TIME);
            else {
                chargedTime = 0;
                EmitSignal (nameof (StopCharge));
            }
        }
    }
    public bool IsBlocking { get { return (currentlyPerformed == null) ? false : currentlyPerformed.Action == ActionType.Block; } }
    public ActionList repeatAction = ActionList.none;
    public ActionList nextAction = ActionList.none;

    public new bool CanMove { get { return (!IsAttacking || currentlyPerformed.Action == ActionType.Block) && MyBody.CanMove; } }
    public bool CanDoAction () { return !IsAttacking && MyBody.CanMove && !isCharging; }
    public bool CanDoAction (ActionList action) { return CanDoAction () && cooldown[action] <= 0; }

    public PlayerAttack_Attack myAttack;
    public PlayerAttack_Block myBlock;

    public override void _Ready () {
        myAttack = GetNode<PlayerAttack_Attack> ("Attack");
        myBlock = GetNode<PlayerAttack_Block> ("Block");
        base._Ready ();
    }

    [Signal] public delegate void StartCooldown (string action, float time);
    [Signal] public delegate void StartCharge (string action, float charge);
    [Signal] public delegate void StopCharge ();
    Godot.Collections.Dictionary<ActionList, float> cooldown =
        new Godot.Collections.Dictionary<ActionList, float> () { { ActionList.left_action, 0 }, { ActionList.right_action, 0 }, { ActionList.dash, 0 },
        };
    Godot.Collections.Dictionary<ActionList, string> actionToPanelName =
        new Godot.Collections.Dictionary<ActionList, string> () { { ActionList.left_action, "LeftHand" }, { ActionList.right_action, "RightHand" }, { ActionList.right_action, "Dash" },
        };

    public enum ActionList {
        none = -1,
        left_action,
        right_action,
        dash,
    }

    public override void _Input (InputEvent _event) {
        if (IsMaster) {
            foreach (ActionList act in Enum.GetValues (typeof (ActionList))) {
                if (act == ActionList.none) continue;
                if (_event.IsActionPressed (act.ToString ())) {
                    if (!CanDoAction (act)) {
                        nextAction = act;
                        return;
                    }
                    if (Network.IsConnectionStarted)
                        this.Rpc (nameof (_StartAction), act); // If connected to a server, the event will be sent to all puppets
                    else
                        _StartAction (act); // If not it will just be performed normally
                    return;
                }
                if (_event.IsActionReleased (act.ToString ())) {
                    if (Network.IsConnectionStarted)
                        this.Rpc (nameof (_ActionReleased), act);
                    else
                        _ActionReleased (act);
                    return;
                }
            }
            if (_event is InputEventMouseMotion eventMouseMotion) {
                MyBody.FacingDirection = GetGlobalMousePosition () - MyBody.GlobalPosition;
            }
        }
    }

    void InitCooldown (ActionList action, float value) {
        EmitSignal (nameof (StartCooldown), actionToPanelName[action], value);
        cooldown[action] = value;
    }

    void CooldownFinished (ActionList action) {
        if (repeatAction == action && CanDoAction (action)) {
            _StartAction (action);
        }
        if (nextAction == action && CanDoAction (action)) {
            _StartAction (action);
            nextAction = ActionList.none;
        }
    }

    [PuppetSync]
    public void _StartAction (ActionList _action) {
        repeatAction = ActionList.none;
        if (_action.ToString ().Contains ("_action")) {
            var weaponData = GameRoot.inventory.equipement.GetAction (_action == ActionList.left_action);
            if (weaponData.Action == ActionType.MultiAttack) {
                LaunchAttack (weaponData, _action);
                repeatAction = _action;
            } else if (weaponData.Action == ActionType.ChargeAttack) {
                myAttack.ChargeAttack (weaponData.AttackData);
                IsCharging = true;
            } else if (weaponData.Action == ActionType.Block) {
                LaunchAttack (weaponData, _action);
            }
        } else if (_action == ActionList.dash) {
            MyBody.StartImpact (MyBody.FacingDirection, 0.2f);
        }
    }

    [PuppetSync]
    public void _ActionReleased (ActionList _action) {
        if (_action == nextAction)
            nextAction = ActionList.none;
        if (_action.ToString ().Contains ("_action")) {
            var weaponData = GameRoot.inventory.equipement.GetAction (_action == ActionList.left_action);
            if (weaponData.Action == ActionType.MultiAttack) {
                repeatAction = ActionList.none;
            } else if (weaponData.Action == ActionType.ChargeAttack && IsCharging) {
                LaunchAttack (weaponData, _action, (chargedTime >= CHARGE_TIME) ? CHARGE_TEMPLATE : null);
                IsCharging = false;
            } else if (weaponData.Action == ActionType.Block)
                myBlock.StopBlock ();
        }
    }

    public void LaunchAttack (WeaponResource weaponData, ActionList action, AttackTemplate template = null) {
        if (weaponData.Action == ActionType.Block)
            myBlock.LaunchBlock (weaponData.AttackData * template);
        else
            myAttack.LaunchAttack (weaponData.AttackData * template);
        currentlyPerformed = weaponData; // Disable all future actions, call _AttackFinished to reenable actions
        if (IsMaster) InitCooldown (action, weaponData.Cooldown);
    }

    public void _AttackFinished () {
        currentlyPerformed = null;
        if (nextAction != ActionList.none && CanDoAction (nextAction)) {
            _StartAction (nextAction);
            nextAction = ActionList.none;
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
                if (IsCharging)
                    inputMovement *= CHARGE_SPEED_MULTIPLIER;
                if (IsBlocking)
                    inputMovement *= BLOCK_SPEED_MULTIPLIER;
                MyBody.NextMovement = inputMovement;
                inputMovement = new Vector2 (0, 0);
            }
            if (Input.IsActionPressed ("interact") && currentInteraction != null)
                currentInteraction.Interact ();
            foreach (var act in cooldown) {
                if (act.Value <= 0)
                    continue;
                cooldown[act.Key] -= delta;
                if (cooldown[act.Key] <= 0) CooldownFinished (act.Key);
            }
        }
        if (isCharging) chargedTime += delta;
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