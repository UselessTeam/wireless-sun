using System;
using Godot;

public class BlandItem : Control {
	Item.ItemStack myStack;
	public void Display (Item.ItemStack stack) {
		myStack = stack;
		((Label) GetNode ("Holder/Label")).Text = stack.size.ToString ();
		((AtlasTexture) ((Sprite) GetNode ("Holder/Item")).Texture).Region = stack.item.data.sprite.GetRect ();
	}

}