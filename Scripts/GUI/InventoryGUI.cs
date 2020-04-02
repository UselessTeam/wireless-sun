using System;
using Godot;

public class InventoryGUI : GUIWindow {
	InventoryBag bag;
	public override void _Ready () {
		bag = (InventoryBag) GetNode ("CenterContainer/InventoryBag");
	}
	public override void Maximise () {
		this.Show ();
	}
	public override void Minimise () {
		this.Hide ();
	}
}