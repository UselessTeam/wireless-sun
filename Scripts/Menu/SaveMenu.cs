using System;
using Godot;

public class SaveMenu : Control {

	public static SaveMenu Instance { get { return instance; } }
	static SaveMenu instance = null;

	public override void _Ready () {
		instance = this;
	}

	void _OnLoadPressed () {
		Global.LoadGameScene ("saveName");
	}

	void _OnNewPressed () {
		Global.MakeNewGameScene ("saveName");
		Global.LoadGameScene ("saveName");
	}

}
