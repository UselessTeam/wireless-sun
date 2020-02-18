using System;
using Godot;

public class ShadowBallAttack : Attack {

    public override void _Ready () {
        DAMAGE = 10;
        base._Ready ();
    }
}