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
			GetNode<Sprite> ("Holder/Item").Texture = slot.item.data.icon;
			GetNode<Sprite> ("Holder/Item").Show ();
		}
		this.Show ();
	}

}