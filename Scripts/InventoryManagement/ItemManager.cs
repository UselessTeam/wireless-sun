using System;
using System.Collections.Generic;
using Godot;
using Item;

public class ItemManager : Node {
	public override void _Ready () {
		Item.Manager.Load();
		QueueFree();
	}
}
