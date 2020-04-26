using System;
using Godot;

public class WeaponResource : EquipementResource {
    [Export] private AttackTemplate template = null;
    [Export] public readonly ActionType Action;
    [Export] private float baseDamage = 0;
    [Export] private float baseRange = 1;
    [Export] private float baseCooldown = 1;

    public float Damage { get { return baseDamage * ((template == null) ? 1 : template.Damage); } }
    public float Range { get { return baseRange * ((template == null) ? 1 : template.Range); } }
    public float Cooldown { get { return baseCooldown * ((template == null) ? 1 : template.Cooldown); } }
    public AttackType Types { get { return ((template == null) ? AttackType.None : template.Type); } }
    public AttackEffect Effects { get { return ((template == null) ? AttackEffect.None : template.Effect); } }

    public AttackTemplate ToAttackTemplate () { return new AttackTemplate (Damage, Range, Cooldown, Types, Effects); }
}

public enum ActionType {
    Attack,
    Block
}