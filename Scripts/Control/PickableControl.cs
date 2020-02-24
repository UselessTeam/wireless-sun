using System;
using Godot;

public class PickableControl : _Control {
	[Export] string item;
	[Export] int amount;

	public void _OnCollisionWithPlayer (KinematicCollision2D collInfo) {
		if (Network.IsConnectionStarted)
			Rpc ("Gather");
		if (IsMaster)
			Gather ();
	}

	[Puppet] public void Gather () {
		//TODO
		Global.inventory.Add(Item.Manager.GetId(item));
		GetParent<Body> ().QueueFree ();
	}
}
