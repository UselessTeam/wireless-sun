using System;
using Godot;

public abstract class _Interactable : Area2D {
    Light2D hoverLight { get { return GetNode<Light2D> ("../HoverLight"); } }
    bool playerIsInRange = false;

    public override void _Ready () {
        this.GetParent<CollisionObject2D> ().Connect ("mouse_entered", this, "_on_mouse_entered");
        this.GetParent<CollisionObject2D> ().Connect ("mouse_exited", this, "_on_mouse_exited");
        this.GetParent<CollisionObject2D> ().Connect ("input_event", this, "_on_input_event");
    }

    public void _on_Area2D_body_entered (PhysicsBody2D _body) {
        _body.GetNode<PC> ("PlayerControl")._InteractEntered (this);
        playerIsInRange = true;
    }
    public void _on_Area2D_body_exited (PhysicsBody2D _body) {
        _body.GetNode<PC> ("PlayerControl")._InteractExited (this);
        playerIsInRange = false;
    }

    public virtual void _on_mouse_entered () {
        hoverLight.Show ();
    }
    public void _on_mouse_exited () {
        hoverLight.Hide ();
    }

    public void _on_input_event (Godot.Object viewport, InputEvent _input, int shape_idx) {
        if (playerIsInRange && _input.IsActionPressed ("ui_select")) {
            Interact ();
        }
    }

    public abstract void Interact ();
}