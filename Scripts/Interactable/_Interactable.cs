using System;
using Godot;

public abstract class _Interactable : Area2D {

    public void _on_Area2D_body_entered (PhysicsBody2D _body) {
        _body.GetNode<PC> ("PlayerControl")._InteractEntered (this);
    }
    public void _on_Area2D_body_exited (PhysicsBody2D _body) {
        _body.GetNode<PC> ("PlayerControl")._InteractExited (this);
    }

    public abstract void Interact ();
}