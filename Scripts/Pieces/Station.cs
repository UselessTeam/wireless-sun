using System;
using System.Collections.Generic;
using Godot;

public class Station : StaticBody2D {

	[Export] public string craftLocation = "";
	private static Dictionary <string, int> CRAFT_SPRITE_POSITION = new Dictionary<string, int> {
		["campfire"] = 0,
		["CraftingTable"] = 1,
		["forge"] = 2,
	};
	public CraftListGUI craftingGUI { get { return GetNode<GUI> ("/root/GUI").crafting; } }

	private bool isGuiDisplayed = false;

	public override void _Ready () {
		Node interaction = GetNode ("Interaction");
		interaction.Connect ("interaction", this, "_on_interaction");
		interaction.Connect ("leave_interaction", this, "_on_leave_interaction");
		// HACK
		int i;
		if (!CRAFT_SPRITE_POSITION.TryGetValue(craftLocation, out i)) {
			i = 3;
		}
		GetNode<Sprite> ("Display/Sprite").RegionRect = new Rect2(96 * i, 0, 96, 96);
	}

	public void _on_interaction () {
		if (!isGuiDisplayed) {
			craftingGUI.Display (craftLocation);
			craftingGUI.Maximise ();
			isGuiDisplayed = true;
		}
	}

	public void _on_leave_interaction () {
		craftingGUI.Minimise ();
		isGuiDisplayed = false;
	}
}
