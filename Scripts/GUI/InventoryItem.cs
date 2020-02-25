using System;
using Godot;

public class InventoryItem : Control {
	public void Display (Item.ItemStack stack) {
		((Label) GetNode ("Label")).Text = stack.size.ToString ();
		((AtlasTexture) ((Sprite) GetNode ("Holder/Item")).Texture).Region = stack.item.data.sprite.GetRect ();
	}
}
