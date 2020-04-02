using System;
using Godot;

public class InventoryItem : Control {
	bool isSelected = false;
	public void Display (Item.ItemStack stack) {
		((Label) GetNode ("Label")).Text = stack.size.ToString ();
		((AtlasTexture) ((Sprite) GetNode ("Holder/Item")).Texture).Region = stack.item.data.sprite.GetRect ();
	}

	public void _on_InventoryItem_mouse_entered () {
		if (!isSelected) GetNode<Sprite> ("Holder/Select").Show ();
	}
	public void _on_InventoryItem_mouse_exited () {
		if (!isSelected) GetNode<Sprite> ("Holder/Select").Hide ();
	}

	public void _on_InventoryItem_gui_input (InputEvent _input) {
		if (_input.IsActionPressed ("ui_select")) {
			if (!isSelected) {
				isSelected = true;
				GetNode<Sprite> ("Holder/Select").Show ();
			} else {
				isSelected = false;
				GetNode<Sprite> ("Holder/Select").Hide ();
			}
		}
	}
}