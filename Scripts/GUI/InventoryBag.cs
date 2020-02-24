using Godot;
using System;

public class InventoryBag : PanelContainer
{

	const int MARGIN = 6;
	const int CELL_SIZE = 24;

	[Export]
	public int columns { get {
			return _columns;
		}
		set{
			_columns = value;
			toUpdate = true;
		}
	}

	private GridContainer cachedGrid = null;

	private bool toUpdate = true;

	private GridContainer GetGrid() {
		if(cachedGrid == null) {
			cachedGrid = (GridContainer)GetNode("Grid");
		}
		return cachedGrid;
	}

	private int _columns;
	public override void _Ready() {
		if (toUpdate) {
			toUpdate = false;
			UpdateSize();
		}
	}

	private void UpdateSize() {
		RectMinSize = new Vector2(columns*(CELL_SIZE+MARGIN) + MARGIN, RectMinSize.y);
		// TODO: Send signal so grid can resize itself
		GetGrid().Columns = columns;
	}
}
