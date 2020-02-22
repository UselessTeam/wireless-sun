using Godot;
public class ChangableSpriteColor : ChangableSprite
{
    [Export]
	private int firstIndex;
	[Export]
	private int totalIndex;

	public override void _Ready () {
        base._Ready();
	}

    public Color color_key { private set; get; } = Color.Color8(0,0,0);

	public void Change(int to_index, int hue) {
		key = to_index;
		Frame = firstIndex + to_index;
		EmitSignal("changed");
	}

	public override void RandomSetup() {
		int new_value = General.rng.Next(totalIndex);
		Change(new_value);
	}
}