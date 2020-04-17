using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public class GameRoot : Node {
	public static Inventory inventory;
	private static GameRoot instance;
	public static GameRoot Instance { get { return instance; } }

	public static string username = "";

	public override void _Ready () {
		instance = this;
		Item.Manager.Load ();
		Craft.Manager.Load ();
	}

	public static void LoadGameScene (string saveName) {
		instance.GetTree ().ChangeScene ("res://Scenes/Beach.tscn"); // We should load the game scene corresponding to saveName
		Save.CurrentSave = saveName;
	}

	public static void LoadMenuScene () {
		Instance.GetNode<GUI> ("/root/GUI").Toggle (GUI.Window.Nothing);
		instance.GetTree ().ChangeScene ("res://Scenes/Menu.tscn");
		Save.CurrentSave = "";
		inventory.InitializeEmpty ();
	}

	public static void BuildNewWorld (string saveName) {
		Save.MakeNewSave (saveName); //Erase any save that has the same name as ours
	}

	public static void _OnGameSceneStarted () {
		if (Save.CurrentSave != "")
			Save.LoadGame ();
	}
}

public static class Save {

	public static string saveLocation = "res://save";
	static string currentSave = "";
	static public string CurrentSave {
		get { return currentSave; }
		set { currentSave = value; }
	}

	static Directory Directory = new Directory ();
	static File File = new File ();

	static string SaveLocation (string saveName) {
		return saveLocation + "/" + saveName + ".save";
	}

	// Scene changing
	public static void MakeNewSave (string saveName) {
		// Create a the map and save it as a scene 
		// TODO
		//Remove any previous save
		if (File.FileExists (SaveLocation (saveName)))
			Directory.Remove (SaveLocation (saveName));
		if (!Directory.DirExists (saveLocation))
			Directory.MakeDir (saveLocation);
	}

	// Save and load game

	public static void SaveGame () {
		var saveGame = new File ();
		saveGame.Open (SaveLocation (currentSave), File.ModeFlags.Write);

		var saveNodes = GameRoot.Instance.GetTree ().GetNodesInGroup ("SaveNodes");
		foreach (Node saveNode in saveNodes) {
			// if (saveNode.GetParent ().Filename.Empty ()) { GD.Print ("control node " + saveNode.Name + " is not an instanced scene, skipped"); continue; }
			saveGame.StoreLine (JSON.Print (saveNode.Call ("MakeSave")));
		}
		saveGame.Close ();
		GD.Print ("Saved game : " + currentSave);

	}
	public static void LoadGame () {
		var saveGame = new File ();
		if (!saveGame.FileExists (SaveLocation (currentSave))) {
			GD.Print ("Loaded empty save");
			return; // No save to load
		}

		// First we delete all the Controls nodes, in order to make sure the game state is clean
		var nodes = GameRoot.Instance.GetTree ().GetNodesInGroup ("ReloadOnSave");
		foreach (Node node in nodes)
			node.QueueFree ();

		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open (SaveLocation (currentSave), File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var line = saveGame.GetLine ();
			// dynamic nodeData = (JSON.Parse (line).Result); //Godot.Collections.Dictionary<string, object>
			var nodeData = JsonConvert.DeserializeObject<Dictionary<string, object>> (saveGame.GetLine ());
			if (nodeData == null) GD.Print ("null");
			// Firstly, we need to check if we need to create the object 
			Node loadNode;
			if (nodeData.ContainsKey ("Filename")) {
				loadNode = (Node) ((PackedScene) ResourceLoader.Load (nodeData["Filename"].ToString ())).Instance ();
				loadNode.Name = nodeData["Name"].ToString ();
				GameRoot.Instance.GetNode (nodeData["Parent"].ToString ()).AddChild (loadNode);
			} else {
				loadNode = GameRoot.Instance.GetNode (nodeData["Path"].ToString ());
			}
			// // Call the node's load function
			if (!loadNode.HasMethod ("LoadData")) {
				GD.Print ("persistent node " + loadNode.Name + " is missing a Load function, skipped");
				continue;
			}
			loadNode.Call ("LoadData", nodeData);
		}
		saveGame.Close ();
		GD.Print ("Loaded save : " + currentSave);
	}
}