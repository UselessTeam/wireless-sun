using System;
using Godot;

public class _GUIItem : Control {
	protected Item.ItemStack myStack;
	public Item.ItemStack MyStack { get { return myStack; } }

	public void Display (Item.ItemStack stack) {
		myStack = stack;
		((Label) GetNode ("Holder/Label")).Text = (stack.size != 0) ? stack.size.ToString () : "";
		if (stack.item == Item.ItemId.NULL)
			GetNode<Sprite> ("Holder/Item").Hide ();
		((AtlasTexture) ((Sprite) GetNode ("Holder/Item")).Texture).Region = stack.item.data.sprite.GetRect ();
		this.Show ();
	}

}