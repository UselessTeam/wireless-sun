using System;
using Godot;

public class InteractionComponent : Area2D {

	[Signal] public delegate void interaction ();
	[Signal] public delegate void leave_interaction ();
	Light2D hoverLight { get { return GetNode<Light2D> ("../Display/HoverLight"); } }
	bool playerIsInRange = false;

	public override void _Ready () {
		GetParent<CollisionObject2D> ().Connect ("mouse_entered", this, "_on_mouse_entered");
		GetParent<CollisionObject2D> ().Connect ("mouse_exited", this, "_on_mouse_exited");
		GetParent<CollisionObject2D> ().Connect ("input_event", this, "_on_input_event");
		Connect ("body_entered", this, "_on_body_entered");
		Connect ("body_exited", this, "_on_body_exited");
	}

	public void _on_body_entered (PhysicsBody2D _body) {
		_body.GetNode<PlayerControl> ("Control")._InteractEntered (this);
		playerIsInRange = true;
	}
	public void _on_body_exited (PhysicsBody2D _body) {
		EmitSignal(nameof(leave_interaction));
		_body.GetNode<PlayerControl> ("Control")._InteractExited (this);
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

	public void Interact () {
		EmitSignal (nameof (interaction));
	}
}
