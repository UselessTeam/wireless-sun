using System;
using Godot;

public class PickableControl : _Control {
	[Export] string item;
	[Export] int amount;

	public void _OnCollisionWithPlayer (KinematicCollision2D collInfo) {
		if (IsMaster) {
			if (Network.IsConnectionStarted)
				Rpc ("Gather");
			else
				Gather ();
		}
	}

	[PuppetSync] public void Gather () {
		GameRoot.inventory.Add (Item.Manager.GetId (item));
		GetParent<Body> ().QueueFree ();
	}

	public override void SaveIn (Godot.Collections.Dictionary<string, object> saveObject) { }
	public override void LoadData (Godot.Collections.Dictionary<string, object> saveObject) { }
}