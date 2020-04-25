using System;
using Godot;

public class WeaponHotbar : PanelContainer {
    public override void _Ready () {
        GameRoot.inventory.Connect ("equipement_change", this, nameof (_on_equipement_change));
        _on_equipement_change ();
    }
    public void _on_equipement_change () {
        GetNode<EquipementItem> ("GridContainer/RightHand").Display (
            GameRoot.inventory.equipement.EquipementList["RightHand"]);
        GetNode<EquipementItem> ("GridContainer/LeftHand").Display (
            GameRoot.inventory.equipement.EquipementList["LeftHand"]);
    }
}