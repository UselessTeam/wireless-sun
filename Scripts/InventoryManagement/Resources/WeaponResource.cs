using System;
using Godot;

public class WeaponResource : EquipementResource {
    [Export (PropertyHint.ResourceType, "AttackRessource")] private AttackResource attackData = null;
    [Export] public readonly ActionType Action;
    [Export (PropertyHint.Flags, "Flag1,Flag2,Flag3")] int Test;

    public float Damage { get { return attackData.Damage; } }
    public float Range { get { return attackData.Range; } }
    public float Cooldown { get { return attackData.Cooldown; } }
    public AttackType Types { get { return attackData.Types; } }
    public AttackEffect Effects { get { return attackData.Effects; } }

    public AttackResource ToAttackRessource () { return attackData; }
}

public enum ActionType {
    Attack,
    Block
}