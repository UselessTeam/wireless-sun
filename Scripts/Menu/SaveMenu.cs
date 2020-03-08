using System;
using Godot;

public class SaveMenu : Control {

	public static SaveMenu Instance { get { return instance; } }
	static SaveMenu instance = null;

	public override void _Ready () {
		instance = this;
	}

	void _OnLoadPressed () {
		Global.LoadGameScene ("");
	}

	void _OnNewPressed () {
		Global.MakeNewGameScene ("");
		Global.LoadGameScene ("");
	}

}
