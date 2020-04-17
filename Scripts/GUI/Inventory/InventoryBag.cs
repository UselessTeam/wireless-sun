using System;
using Godot;

public class InventoryBag : PanelContainer {

	const int MARGIN = 6;
	const int CELL_SIZE = 24;

	[Export]
	public int columns { get; protected set; }

	private InventoryGrid cachedGrid = null;

	private InventoryGrid GetGrid () {
		if (cachedGrid == null) {
			cachedGrid = (InventoryGrid) GetNode ("Grid");
		}
		return cachedGrid;
	}

	public override void _Ready () {
		SetColumns (4);
		GameRoot.inventory.Connect ("inventory_change", this, nameof (_on_inventory_change));
		_on_inventory_change ();
	}

	private void SetColumns (int value) {
		columns = value;
		RectMinSize = new Vector2 (columns * (CELL_SIZE + MARGIN) + MARGIN, RectMinSize.y);
		// TODO: Send signal so grid can resize itself
		GetGrid ().Columns = columns;
	}

	public void _on_inventory_change () {
		GetGrid ().Display (GameRoot.inventory.inventory);
	}
}