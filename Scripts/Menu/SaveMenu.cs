using System;
using Godot;

public class SaveMenu : Control {
	[Export] PackedScene loadButtonPackedScene;

	public static SaveMenu Instance { get { return instance; } }
	static SaveMenu instance = null;

	public Control MainSaveMenu;
	public Control ChooseNewName { get { return GetNode<Control> ("ChooseNewName"); } }

	public string newName;

	public override void _Ready () {
		instance = this;
		MainSaveMenu = GetNode<Control> ("MainSaveMenu");
		Connect ("draw", this, nameof (_OnDraw));
	}

	public void _OnDraw () {
		ChooseNewName.GetNode<Label> ("NameError").Text = "";
		for (int i = 2; i < MainSaveMenu.GetChildCount (); i++) {
			MainSaveMenu.GetChild (i).QueueFree ();
		}
		foreach (var save in Save.GetSaveList ()) {
			var loadButton = (LoadButton) loadButtonPackedScene.Instance ();
			loadButton.Name = save;
			loadButton.Text = save;
			MainSaveMenu.AddChild (loadButton);
			loadButton.Connect (nameof (LoadButton.LoadPressed), this, nameof (_OnLoadPressed));
		}
	}

	public void _OnLoadPressed (string name) {
		GameRoot.LoadGameScene (name);
	}

	void _OnNewPressed () {
		MainSaveMenu.Hide ();
		ChooseNewName.Show ();
	}

	void _OnNewNameEdit (string line) {
		newName = line;
	}
	void _OnMakeSavePressed () {
		if (newName == "")
			ChooseNewName.GetNode<Label> ("NameError").Text = "You need to choose a name";

		else if (!NameIsNew ())
			ChooseNewName.GetNode<Label> ("NameError").Text = "Name already exists";
		else {
			GameRoot.BuildNewWorld (newName);
			ChooseNewName.Hide ();
			MainSaveMenu.Show ();
			_OnDraw ();
		}
	}
	void _OnCancelPressed () {
		ChooseNewName.Hide ();
		MainSaveMenu.Show ();
	}
	bool NameIsNew () {
		foreach (var save in Save.GetSaveList ()) {
			if (save.ToLower () == newName.ToLower ())
				return false;
		}
		return true;

	}

}