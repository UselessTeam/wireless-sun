using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public class GameRoot : Node {
	public static Inventory inventory;
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

	public static string saveLocation = "res://save";
	static string currentSave = "";
	static public string CurrentSave {
		get { return currentSave; }
		set { currentSave = value; }
	}

	static Godot.Directory Directory = new Directory ();
	static Godot.File File = new Godot.File ();

	public static List<string> GetSaveList () {
		var list = new List<string> ();
		if (!Directory.DirExists (saveLocation))
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
		return saveLocation + "/" + saveName + ".save";
	}

	// Scene changing
	public static void MakeNewSave (string saveName) {
		// Create a the map and save it as a scene 
		// TODO
		//Remove any previous save
		if (File.FileExists (SavePath (saveName)))
			Directory.Remove (SavePath (saveName));
		if (!Directory.DirExists (saveLocation))
			Directory.MakeDir (saveLocation);
		var saveGame = new File ();
		saveGame.Open (SavePath (saveName), File.ModeFlags.Write);
		saveGame.Close ();
	}

	// Save and load game

	public static void SaveGame () {
		var saveGame = new File ();
		saveGame.Open (SavePath (currentSave), File.ModeFlags.Write);

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
		if (!saveGame.FileExists (SavePath (currentSave))) {
			GD.Print ("Loaded empty save");
			return; // No save to load
		}
		saveGame.Open (SavePath (currentSave), File.ModeFlags.Read);

		if (saveGame.GetLen () == 0)
			return;

		// First we delete all the Controls nodes, in order to make sure the game state is clean
		var nodes = GameRoot.Instance.GetTree ().GetNodesInGroup ("ReloadOnSave");
		foreach (Node node in nodes)
			node.QueueFree ();

		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var line = saveGame.GetLine ();
			// dynamic nodeData = (JSON.Parse (line).Result); //Godot.Collections.Dictionary<string, object>
			var nodeData = JsonConvert.DeserializeObject<Godot.Collections.Dictionary<string, object>> (saveGame.GetLine ());
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
		saveGame.Close ();
		GD.Print ("Loaded save : " + currentSave);
	}
}