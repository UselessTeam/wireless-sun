using System;
using Godot;

public class InventoryGUI : GUIWindow {
	InventoryBag bag;
	public override void _Ready () {
		bag = (InventoryBag) GetNode ("InventoryBag");
		GameRoot.inventory.Connect ("equipement_change", this, nameof (_on_equipement_change));
		_on_equipement_change ();
	}
	public override void Maximise () {
		this.Show ();
	}
	public override void Minimise () {
		this.Hide ();
	}
	public void _on_equipement_change () {
		GetNode<HoverableItem> ("EquipedItem").Display (GameRoot.inventory.equipement.rightHand);
	}
}