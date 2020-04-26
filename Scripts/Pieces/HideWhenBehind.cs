using Godot;
using System;

public class HideWhenBehind : Area2D
{
	private static float time = 0.02f;
	private static Color shown = new Color(1, 1, 1, 1);
	private static Color obscured = new Color(1, 1, 1, 0.2f);
	private static Color invisible = new Color(1, 1, 1, 0);
	
	private Sprite sprite;
	private Tween tween;
	[Export] public bool isObscured = true;
	public Color hidden { get {return isObscured ? obscured : invisible; } }

	public override void _Ready()
	{
		sprite = (Sprite)GetNode("Sprite");
		tween = (Tween)GetNode("Tween");
	}

	private void _on_body_entered(object body)
	{
		HidePart();
	}
	
	private void _on_body_exited(object body)
	{
		ShowPart();
	}

	private void HidePart() {
		tween.InterpolateProperty(sprite, "modulate",
				shown, hidden, time,
				Tween.TransitionType.Linear, Tween.EaseType.InOut);
		tween.Start();
	}

	private void ShowPart() {
		tween.InterpolateProperty(sprite, "modulate",
				hidden, shown, time,
				Tween.TransitionType.Linear, Tween.EaseType.InOut);
		tween.Start();
	}
}
