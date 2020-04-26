using System;
using Godot;

public class WeaponResource : EquipementResource {
    [Export] public ActionType action;
    [Export] public float damage = 0;
    [Export] public float range = 1;
    [Export] public AttackEffet[] effects = new AttackEffet[0];

    // public WeaponResource (float armor = 0, float damage = 20, float range = 0.4f) {
    //     this.armor = armor;
    //     this.damage = damage;
    //     this.range = range;
    // }

    public static WeaponResource EmptyHand = new WeaponResource ();
}

public enum ActionType {
    Attack,
    Block
}

public enum AttackEffet {
    Fire
}