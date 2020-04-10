using System;
using System.Collections.Generic;
using Godot;

public class MinableControl : _Control {
	[Export] public string[] items;
	[Export] public ushort[] quantities;
	[Export] public float dropRadius = 15.0f;

	// public void AddStack (string item, ushort quantity) { this.items.Add (item); this.quantities.Add (quantity); }

	public new void _OnDied () {
		for (int i = 0; i < items.Length; i++) {
			var item = items[i];
			// var quantity = quantities[i];
			Body itemBody = Item.Builder.MakeBody (item, 1);
			itemBody.Position = MyBody.Position + _Spawner.GenerateSpawnPosition (dropRadius);
			MyBody.GetParent ().AddChild (itemBody);
		}
		base._OnDied ();
	}
}