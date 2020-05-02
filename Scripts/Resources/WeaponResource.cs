using System;
using Godot;
using Item;

public class WeaponResource : EquipementResource {
    [Export (PropertyHint.ResourceType, "AttackRessource")] private AttackResource attackData = null;
    [Export] public readonly ActionType Action;
    [Export] AttackTemplate weaponTemplate;
    [Export] AttackTemplate materialTemplate;
    [Export] AttackTemplate bonusTemplate;
    [Export (PropertyHint.Flags, "Flag1,Flag2,Flag3")] int Test;

    public float Damage { get { return attackData.Damage; } }
    public float Range { get { return attackData.Range; } }
    public float Cooldown { get { return attackData.Cooldown; } }
    public AttackType Types { get { return attackData.Types; } }
    public AttackEffect Effects { get { return attackData.Effects; } }

    public AttackResource AttackData { get { return attackData; } }

    public override void _Init (string name, ItemId id) {
        base._Init (name, id);
        attackData.ApplyTemplate (weaponTemplate);
        attackData.ApplyTemplate (materialTemplate);
        attackData.ApplyTemplate (bonusTemplate);
        weaponTemplate = null;
        materialTemplate = null;
        bonusTemplate = null;
    }
}

public enum ActionType {
    MultiAttack,
    ChargeAttack,
    Block
}