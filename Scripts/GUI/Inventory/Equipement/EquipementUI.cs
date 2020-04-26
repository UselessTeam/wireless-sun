using System;
using Godot;

public class EquipementUI : PanelContainer {
    public override void _Ready () {
        GameRoot.inventory.Connect ("equipement_change", this, nameof (_on_equipement_change));
        _on_equipement_change ();
    }

    public void _on_equipement_change () {
        foreach (var equipment in GameRoot.inventory.equipement.EquipementList) {
            if (equipment.Key.Contains ("Hand")) continue;
            GetNode<EquipementItem> ("GridContainer/" + equipment.Key).Display (
                equipment.Value);
        }
    }
}