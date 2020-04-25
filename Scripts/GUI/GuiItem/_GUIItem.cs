using System;
using Godot;

public class _GUIItem : Control {
	protected Item.ItemSlot mySlot;
	public Item.ItemSlot MySlot { get { return mySlot; } set { mySlot = value; } }

	public virtual void Display (Item.ItemSlot slot) {
		mySlot = slot;
		((Label) GetNode ("Holder/Label")).Text = slot.GetLabel ();
		if (slot.item == Item.ItemId.NULL)
			GetNode<Sprite> ("Holder/Item").Hide ();
		else {
			((AtlasTexture) ((Sprite) GetNode ("Holder/Item")).Texture).Region = slot.item.data.sprite.GetRect ();
			GetNode<Sprite> ("Holder/Item").Show ();
		}
		this.Show ();
	}

}