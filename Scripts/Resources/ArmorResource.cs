using System;
using Godot;

public class ArmorResource : Resource {
    [Export] AttackType helper;
    [Export] public Godot.Collections.Dictionary<AttackType, float> armorValues;

    public float ApplyDamage (AttackType type, float damage = 1) { return (armorValues.ContainsKey (type)) ? damage * armorValues[type] : damage; }

    public static float CombineAndApplyDamage (ArmorResource[] armors, AttackType type, float damage = 1) {
        foreach (var armor in armors)
            damage *= armor.ApplyDamage (type);
        return damage;
    }
    public static ArmorResource CombineArmors (ArmorResource[] armors) {
        ArmorResource result = new ArmorResource ();
        foreach (AttackType type in Enum.GetValues (typeof (AttackType)))
            result.armorValues[type] = CombineAndApplyDamage (armors, type);
        return result;
    }
}