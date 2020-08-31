// using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;

public class GameRoot : Node {
	public static Inventory inventory;
	public static Stats.PlayerStats playerStats;

	private static GameRoot instance;
	public static GameRoot Instance { get { return instance; } }

	public static GUI _GUI { get { return Instance.GetNode<GUI> ("/root/GUI"); } }

	public static string username = "";

	[Signal] public delegate void GameplayStarted ();

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
		_GUI.Toggle (GUI.Window.Nothing);
		instance.GetTree ().ChangeScene ("res://Scenes/Menu.tscn");
		Save.CurrentSave = "";
		inventory.InitializeEmpty ();
		_GUI.GameplayEnd ();

	}

	public static void BuildNewWorld (string saveName) {
		Save.MakeNewSave (saveName); //Erase any save that has the same name as ours
	}

	public static void GameplayStart () {
		if (Save.CurrentSave != "")
			Save.LoadGame ();
	}

	public static void GameplayReady () {
		Instance.EmitSignal (nameof (GameplayStarted));
	}
}

public static class Save {

	public static string saveLocation = "user://save";
	static string currentSave = "";
	static public string CurrentSave {
		get { return currentSave; }
		set { currentSave = value; }
	}

	static Godot.Directory dir = new Directory ();
	static Godot.File file = new Godot.File ();

	public static Array<string> GetSaveList () {
		var list = new Array<string> ();
		if (!dir.DirExists (saveLocation))
			return list;
		var savesDir = new Directory ();
		savesDir.Open (saveLocation);
		savesDir.ListDirBegin (true, true);
		string itemPath;
		do {
			itemPath = savesDir.GetNext ();
			if (itemPath.EndsWith (".save"))
				list.Add (itemPath.Remove (itemPath.Length - ".save".Length));
		} while (itemPath != "");
		return list;
	}

	static string SavePath (string saveName) {
		GD.Print ("Save Path is: " + OS.GetUserDataDir ());
		return saveLocation + "/" + saveName + ".save";
	}

	// Scene changing
	public static void MakeNewSave (string saveName) {
		// Create a the map and save it as a scene 
		if (dir.FileExists (SavePath (saveName)))
			GD.PrintErr ("Save already exists");
		if (!dir.DirExists (saveLocation)) {
			GD.Print ("Creating save folder");
			dir.Open ("user://");
			dir.MakeDir (saveLocation);
			GD.Print ("It is created : ", dir.DirExists (saveLocation));
		}
		file.Open (SavePath (saveName), File.ModeFlags.Write);
		file.Close ();
	}

	public static void DeleteSave (string saveName) {
		if (dir.FileExists (SavePath (saveName)))
			dir.Remove (SavePath (saveName));
		else
			GD.PrintErr ("Trying to delete non existing save : ", saveName);
	}

	// Save and load game
	public static void SaveGame () {
		file.Open (SavePath (currentSave), File.ModeFlags.Write);

		var saveNodes = GameRoot.Instance.GetTree ().GetNodesInGroup ("SaveNodes");
		foreach (Node saveNode in saveNodes) {
			// if (saveNode.GetParent ().Filename.Empty ()) { GD.Print ("control node " + saveNode.Name + " is not an instanced scene, skipped"); continue; }
			file.StoreLine (JSON.Print (saveNode.Call ("MakeSave")));
		}
		file.Close ();
		GD.Print ("Saved game : " + currentSave);

	}
	public static void LoadGame () {
		if (!file.FileExists (SavePath (currentSave))) {
			GD.Print ("Loaded empty save");
			return; // No save to load
		}
		file.Open (SavePath (currentSave), File.ModeFlags.Read);

		if (file.GetLen () == 0)
			return;

		// First we delete all the Controls nodes, in order to make sure the game state is clean
		var nodes = GameRoot.Instance.GetTree ().GetNodesInGroup ("ReloadOnSave");
		foreach (Node node in nodes)
			node.QueueFree ();

		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		while (file.GetPosition () < file.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var line = saveGame.GetLine ();
			// dynamic nodeData = (JSON.Parse (line).Result); //Godot.Collections.Dictionary<string, object>
			var nodeData = JsonConvert.DeserializeObject<Godot.Collections.Dictionary<string, object>> (file.GetLine ());
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
			if (loadNode.HasMethod ("LoadData")) {
				loadNode.Call ("LoadData", nodeData);
			} else if (loadNode is IPiece) {
				(loadNode as IPiece).LoadData (nodeData);
			} else {
				GD.Print ("persistent node " + loadNode.Name + " is missing a Load function, skipped");

			}
		}
		file.Close ();
		GD.Print ("Loaded save : " + currentSave);
	}
}
