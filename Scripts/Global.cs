using System;
using Godot;

public class Global : Node {
	public static Inventory inventory;
	public static Global global;
	public static string username = "";

	public override void _Ready () {
		global = this;
		Item.Manager.Load ();
	}

	// Scene changing
	public static void MakeNewGameScene (string saveName) { }

	public static void LoadGameScene (string saveName) {
		global.GetTree ().ChangeScene ("res://Scenes/Beach.tscn");
		LoadGame ("savename");
	}

	public static void LoadMenuScene () {
		global.GetTree ().ChangeScene ("res://Scenes/Menu.tscn");
	}

	// Save and load game

	public static void SaveGame (string saveName) {
		var saveGame = new File ();
		saveGame.Open ("res://" + saveName + ".save", File.ModeFlags.Write);

		var saveNodes = global.GetTree ().GetNodesInGroup ("Persistent");
		foreach (Node saveNode in saveNodes) {
			if (saveNode.Filename.Empty ()) {
				GD.Print (String.Format ("persistent node '{0}' is not an instanced scene, skipped", saveNode.Name));
				continue;
			}

			// Call the node's save function
			if (!saveNode.HasMethod ("MakeSave")) {
				GD.Print (String.Format ("persistent node '{0}' is missing a MakeSave() function, skipped", saveNode.Name));
				continue;
			}
			var nodeData = saveNode.Call ("MakeSave");

			// Store the save dictionary as a new line in the save file
			saveGame.StoreLine (JSON.Print (nodeData));
		}
		saveGame.Close ();
		GD.Print ("Saved game : " + saveName);

	}
	public static void LoadGame (string saveName) {
		var saveGame = new File ();
		if (!saveGame.FileExists ("res://" + saveName + ".save")) {
			GD.Print ("Loaded empty save");
			return; // No save to load
		}

		// First we delete all the Persistent nodes, in order to make sure the game state is clean
		var saveNodes = global.GetTree ().GetNodesInGroup ("Persistent");
		foreach (Node saveNode in saveNodes)
			saveNode.QueueFree ();

		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("res://" + saveName + ".save", File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			var line = saveGame.GetLine ();
			GD.Print (line);
			var nodeData = (Godot.Collections.Dictionary<string, object>) JSON.Parse (line).Result;

			// Firstly, we need to create the object and add it to the tree and set its position.
			var newObjectScene = (PackedScene) ResourceLoader.Load (nodeData["Filename"].ToString ());
			var newObject = (Node) newObjectScene.Instance ();
			global.GetNode (nodeData["Parent"].ToString ()).AddChild (newObject);

			// Call the node's load function
			if (!newObject.HasMethod ("LoadData")) {
				GD.Print (String.Format ("persistent node '{0}' is missing a Load function, skipped", newObject.Name));
				continue;
			}
			newObject.Call ("LoadData", nodeData);
		}

		saveGame.Close ();
		GD.Print ("Loaded save : " + saveName);
	}
}
