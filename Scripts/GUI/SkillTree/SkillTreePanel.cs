using System;
using Godot;

public class SkillTreePanel : GUIWindow {
	public static float ZOOM_VALUE = 1.3f;
	public static int MAX_ZOOM = 6;

	bool holdingMouse = false;
	Control treeRoot;
	int currentZoom = 1;

	[Signal] public delegate void UpdateTree ();

	public override void _Ready () {
		Connect ("gui_input", this, nameof (GuiInput));
		treeRoot = GetNode<Control> ("TreeRoot");
	}

	private Vector2 BindScreenPosition (Vector2 position, Vector2 MaxPosition, Vector2 MinPosition) {
		Vector2 boundPosition = position;
		if (boundPosition.x > MaxPosition.x)
			boundPosition.x = MaxPosition.x;
		if (boundPosition.x < MinPosition.x)
			boundPosition.x = MinPosition.x;
		if (boundPosition.y > MaxPosition.y)
			boundPosition.y = MaxPosition.y;
		if (boundPosition.y < MinPosition.y)
			boundPosition.y = MinPosition.y;
		return boundPosition;
	}

	public void GuiInput (InputEvent _input) {
		if (_input.IsActionPressed ("ui_select")) {
			holdingMouse = true;
		} else if (_input.IsActionReleased ("ui_select")) {
			holdingMouse = false;
		} else if (_input is InputEventMouseMotion eventMouseMotion && holdingMouse) {
			treeRoot.RectPosition += eventMouseMotion.Relative;
			// Vector2 ZoomAreaSize = treeRoot.RectSize / Mathf.Pow (ZOOM_VALUE, currentZoom - 1);
			// treeRoot.RectPosition = BindScreenPosition (treeRoot.RectPosition, (treeRoot.RectSize - ZoomAreaSize), -(treeRoot.RectSize - ZoomAreaSize));
		} else if (_input.IsActionPressed ("zoom_in") && currentZoom < MAX_ZOOM) {
			treeRoot.RectPosition -= GetLocalMousePosition () * (ZOOM_VALUE - 1);
			treeRoot.RectScale *= ZOOM_VALUE;
			currentZoom += 1;
		} else if (_input.IsActionPressed ("zoom_out") && currentZoom > 1) {
			treeRoot.RectPosition += GetLocalMousePosition () * (ZOOM_VALUE - 1);
			treeRoot.RectScale /= ZOOM_VALUE;
			currentZoom -= 1;
		}
	}

	public override void Maximise () {
		Show ();
		EmitSignal (nameof (UpdateTree));
	}
	public override void Minimise () { Hide (); }

}
