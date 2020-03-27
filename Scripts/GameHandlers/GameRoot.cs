using Godot;

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
		instance.GetTree ().ChangeScene ("res://Scenes/Menu.tscn");
		Save.CurrentSave = "";
	}

	public static void BuildNewWorld (string saveName) {
		Save.MakeNewSave (saveName);
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

		var saveNodes = GameRoot.Instance.GetTree ().GetNodesInGroup ("Persistent");
		foreach (Node saveNode in saveNodes) {
			if (saveNode.Filename.Empty ()) {
				GD.Print ("persistent node " + saveNode.Name + " is not an instanced scene, skipped");
				continue;
			}

			// Call the node's save function
			if (!saveNode.HasMethod ("MakeSave")) {
				GD.Print ("persistent node " + saveNode.Name + " is missing a MakeSave() function, skipped");
				continue;
			}
			var nodeData = saveNode.Call ("MakeSave");

			// Store the save dictionary as a new line in the save file
			saveGame.StoreLine (JSON.Print (nodeData));
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

		// First we delete all the Persistent nodes, in order to make sure the game state is clean
		var saveNodes = GameRoot.Instance.GetTree ().GetNodesInGroup ("Persistent");
		foreach (Node saveNode in saveNodes)
			saveNode.QueueFree ();

		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open (SaveLocation (currentSave), File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			var line = saveGame.GetLine ();
			GD.Print (line);
			GD.Print (JSON.Print (JSON.Parse (line).Result));
			dynamic nodeData = (JSON.Parse (line).Result); //Godot.Collections.Dictionary<string, object>
			// Firstly, we need to create the object and add it to the tree and set its position.
			var newObjectScene = (PackedScene) ResourceLoader.Load (nodeData["Filename"].ToString ());
			var newObject = (Node) newObjectScene.Instance ();
			GameRoot.Instance.GetNode (nodeData["Parent"].ToString ()).AddChild (newObject);

			// Call the node's load function
			if (!newObject.HasMethod ("LoadData")) {
				GD.Print ("persistent node " + newObject.Name + " is missing a Load function, skipped");
				continue;
			}
			newObject.Call ("LoadData", nodeData);
		}

		saveGame.Close ();
		GD.Print ("Loaded save : " + currentSave);
	}
}