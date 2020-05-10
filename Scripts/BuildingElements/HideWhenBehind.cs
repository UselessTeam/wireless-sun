using Godot;
using System;

public class HideWhenBehind : Area2D
{
    private static float time = 0.15f;
    private static Color shown = new Color(1, 1, 1, 1);
    private static Color obscured = new Color(1, 1, 1, 0.3f);
    private static Color invisible = new Color(1, 1, 1, 0);

    private Tween tween;

    [Export] public bool isObscured = true;
    public Color hidden { get {return isObscured ? obscured : invisible; } }

    public override void _Ready()
    {
        tween = (Tween)GetNode("Tween");
        Connect ("body_entered", this, "_on_body_entered");
        Connect ("body_exited", this, "_on_body_exited");
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
        tween.InterpolateProperty(this, "modulate",
                shown, hidden, time,
                Tween.TransitionType.Linear, Tween.EaseType.InOut);
        tween.Start();
    }

    private void ShowPart() {
        tween.InterpolateProperty(this, "modulate",
                hidden, shown, time,
                Tween.TransitionType.Linear, Tween.EaseType.InOut);
        tween.Start();
    }
}
