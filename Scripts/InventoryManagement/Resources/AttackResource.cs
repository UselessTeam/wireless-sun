using Godot;

public class AttackResource : Resource {
    [Export] public AttackTemplate template = null;
    [Export] private float baseDamage = 0;
    [Export] private float baseRange = 1;
    [Export] private float baseCooldown = 1;

    public float Damage { get { return baseDamage * ((template == null) ? 1 : template.Damage); } }
    public float Range { get { return baseRange * ((template == null) ? 1 : template.Range); } }
    public float Cooldown { get { return baseCooldown * ((template == null) ? 1 : template.Cooldown); } }
    public AttackType Types { get { return ((template == null) ? AttackType.None : template.Type); } }
    public AttackEffect Effects { get { return ((template == null) ? AttackEffect.None : template.Effect); } }

    public AttackResource () {}

    // public AttackRessource (float damage, float range, float cooldown, AttackType type, AttackEffect effect) {
    //     this.Damage = damage;
    //     this.Range = range;
    //     this.Cooldown = cooldown;
    //     this.Types = type;
    //     this.Effects = effect;
    // }
}