using System;
using Godot;

public class PickableControl : ControlComponent {
	[Export] public string item;
	[Export] public ushort quantity = 1;

	public override void _Ready () {
		base._Ready ();
		GetNode<MovementComponent> ("../Movement").Connect (nameof (MovementComponent.BodyCollision), this, "_OnCollisionWithPlayer");
	}

	public void SetStack (string item, ushort quantity) {
		this.item = item;
		this.quantity = quantity;
		GetNode<Sprite> ("Sprite").Texture = Item.Manager.GetId (item).data.icon;
	}

	public void _OnCollisionWithPlayer (KinematicCollision2D collInfo) {
		if (IsMaster) {
			if (Network.IsConnectionStarted)
				Rpc ("Gather");
			else
				Gather ();
		}
	}

	[PuppetSync] public void Gather () {
		GameRoot.inventory.Add (Item.Manager.GetId (item), quantity);
		GetParent<KinematicPiece> ().QueueFree ();
	}

	public new Godot.Collections.Dictionary<string, object> MakeSave () {
		var saveObject = base.MakeSave ();
		saveObject["Item"] = item;
		saveObject["Quantity"] = (int) quantity;
		return saveObject;
	}

	public new void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		base.LoadData (saveObject);
		SetStack (saveObject["Item"].ToString (), Convert.ToUInt16 (saveObject["Quantity"]));
	}
}