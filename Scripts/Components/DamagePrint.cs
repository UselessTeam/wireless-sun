using System;
using Godot;

public class DamagePrint : Label {
	static float SHOW_TIME = 1.2f; //How log can we read the Label

	Tween MyTween;
	public override void _Ready () {
		MyTween = new Tween ();
		AddChild (MyTween);

	}

	public void ShowDamage (float damage) {
		Text = "-" + ((int) damage).ToString ();
		MyTween.InterpolateProperty (this, "modulate:a", 1, 0, SHOW_TIME, Tween.TransitionType.Quart);
		MyTween.Start ();
	}

}
