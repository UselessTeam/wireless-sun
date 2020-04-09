using System;
using Godot;

public class HealthBar : TextureProgress {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    Health MyHealth { get { return GetParent ().GetParent<Health> (); } }
    public override void _Ready () {
        MaxValue = MyHealth.MAX_HP;
        Value = MyHealth.MAX_HP;
        MyHealth.Connect ("hp_changed", this, "OnHpChanged");
    }

    void OnHpChanged (float _HP) { Value = _HP; }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}