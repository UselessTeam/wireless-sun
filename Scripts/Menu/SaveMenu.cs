using System;
using Godot;

public class SaveMenu : Control {

	public static SaveMenu Instance { get { return instance; } }
	static SaveMenu instance = null;

	public override void _Ready () {
		instance = this;
	}

	void _OnLoadPressed () {
		GameRoot.LoadGameScene ("saveName");
	}

	void _OnNewPressed () {
		GameRoot.BuildNewWorld ("saveName");
		GameRoot.LoadGameScene ("saveName");
	}

}