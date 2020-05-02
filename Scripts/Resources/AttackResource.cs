using Godot;

public class AttackResource : Resource {
    [Export] private float baseDamage = 0;
    [Export] private float baseKnockback = 0; //Represents the time (in 1/100 sec) suring which you are knocked back (thus proportinal to distance)
    [Export] private float baseRange = 1;
    [Export] private float baseCooldown = 1;
    [Export] public AttackType types;
    [Export] public AttackEffect effects;

    public float Damage { get { return baseDamage; } }
    public float Range { get { return baseRange; } }
    public float Knockback { get { return baseKnockback; } }
    public float Cooldown { get { return baseCooldown; } }
    public AttackType Types { get { return types; } }
    public AttackEffect Effects { get { return effects; } }

    public AttackResource () {}

    public AttackResource (float damage, float knockback, float range, float cooldown) {
        baseDamage = damage;
        baseKnockback = knockback;
        baseRange = range;
        baseCooldown = cooldown;
    }

    public void ApplyTemplate (AttackTemplate template) {
        if (template == null)
            return;
        baseDamage *= template.Damage;
        baseKnockback *= template.Knockback;
        baseRange *= template.Range;
        baseCooldown *= template.Cooldown;
        types |= template.Type;
        effects |= template.Effect;
    }

    public static AttackResource operator * (AttackResource resource, AttackTemplate template) {
        if (template == null)
            return resource;
        var returnVal = new AttackResource (resource.Damage * template.Damage,
            resource.Knockback * template.Knockback,
            resource.Range * template.Range,
            resource.Cooldown * template.Cooldown);
        returnVal.types = resource.types | template.Type;
        returnVal.effects = resource.effects | template.Effect;
        return returnVal;
    }

    // public AttackResource (float damage, float range, float cooldown, AttackType type, AttackEffect effect) {
    //     this.Damage = damage;
    //     this.Range = range;
    //     this.Cooldown = cooldown;
    //     this.Types = type;
    //     this.Effects = effect;
    // }
}