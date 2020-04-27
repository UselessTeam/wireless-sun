using System;
using System.Collections.Generic;
using Godot;

public class MinableControl : ControlComponent {
	[Export] public string[] items;
	[Export] public int[] quantities;
	[Export] public float dropRadius = 15.0f;

	// public void AddStack (string item, ushort quantity) { this.items.Add (item); this.quantities.Add (quantity); }

	public new void _OnDied () {
		for (int i = 0; i < items.Length; i++) {
			var item = items[i];
			Body itemBody = Item.Builder.MakeBody (item, (ushort) quantities[i]);
			itemBody.Position = MyBody.Position + SpawnerComponent.GenerateSpawnPosition (dropRadius);
			MyBody.GetParent ().AddChild (itemBody);
		}
		base._OnDied ();
	}
}