using System;
using Godot;

public class AttackTemplate : Resource {
    [Export] private AttackTemplate template = null;
    [Export] private float damage = 1;
    [Export] private float knockback = 1;
    [Export] private float range = 1;
    [Export] private float cooldown = 1;
    [Export] private AttackType type;
    [Export] private AttackEffect effect;

    public float Damage { get { return damage * ((template == null) ? 1 : template.Damage); } }
    public float Range { get { return range * ((template == null) ? 1 : template.Range); } }
    public float Knockback { get { return knockback * ((template == null) ? 1 : template.Knockback); } }
    public float Cooldown { get { return cooldown * ((template == null) ? 1 : template.Cooldown); } }
    public AttackType Type { get { return type | ((template == null) ? AttackType.None : template.Type); } }
    public AttackEffect Effect { get { return effect | ((template == null) ? AttackEffect.None : template.Effect); } }

    public AttackTemplate () {}

    public AttackTemplate (float damage = 1, float knockback = 1, float range = 1, float cooldown = 1, AttackType type = AttackType.None, AttackEffect effect = AttackEffect.None) {
        this.damage = damage;
        this.knockback = knockback;
        this.range = range;
        this.cooldown = cooldown;
        this.type = type;
        this.effect = effect;
    }
}

[Flags] public enum AttackType {
    None = 0,
    Sword = 1,
    Axe = 2,
    Spear = 4,
    Pickaxe = 8,
    Fire = 16
}

[Flags] public enum AttackEffect {
    None = 0,
    Burn = 1,
    Freeze = 2,
    Bleed = 4,
    Stun = 8
}