using System;
using Godot;

public class HealthBar : TextureProgress {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    HealthComponent MyHealth { get { return GetParent ().GetParent<HealthComponent> (); } }
    public override void _Ready () {
        MyHealth.Connect (nameof (HealthComponent.HpChanged), this, "OnHpChanged");

    }

    void OnHpChanged (float _HP) {
        MaxValue = MyHealth.MaxHp;
        Value = _HP;
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}