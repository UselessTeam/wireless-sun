using System;
using Godot;

public class Attack : AnimatedSprite {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {

    }

    public void _OnAttackFinished () {
        Hide ();
        Stop ();
        GetFather<PC> ("PC").isAttacking = false;
        // GetNode<SFX> ("SFX").Stop ();
    }

    public void _StartAttack () {
        Show ();
        Play ("straight");
        GetNode<SFX> ("SFX").Play (0);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }
}