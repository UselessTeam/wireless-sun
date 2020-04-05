using System;
using Godot;

public class MinableControl : _Control {
	[Export] public string item;
	[Export] public ushort quantity = 1;

	public void SetStack (string item, ushort quantity) { this.item = item; this.quantity = quantity; }

	public override void SaveIn (Godot.Collections.Dictionary<string, object> saveObject) { }
	public override void LoadData (Godot.Collections.Dictionary<string, object> saveObject) { }

}