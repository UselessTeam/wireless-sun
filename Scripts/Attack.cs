using System;
using Godot;

public class Attack : AnimatedSprite {

    private float DISTANCE_TO_PLAYER = 15;

    public override void _Ready () {

    }

    public void _OnAttackFinished () {
        Hide ();
        Stop ();
        GetFather<PC> ("PC").isAttacking = false;
        // GetNode<SFX> ("SFX").Stop ();
    }

    public float OrientedAngle (Vector2 U, Vector2 V) {
        float alpha = U.Cross (V);
        return U.AngleTo (V) * alpha / Math.Abs (alpha);
    }

    public void _StartAttack (Vector2 direction) {
        Position = DISTANCE_TO_PLAYER * direction;
        Show ();
        if (direction.x == 0 || direction.y == 0) {
            Rotation = direction.Angle ();
            Play ("straight");
        } else {
            // Rotation = OrientedAngle (direction, new Vector2 (1, -1));
            Play ("diagonal");
        }
        GetNode<SFX> ("SFX").Play (0);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }
}