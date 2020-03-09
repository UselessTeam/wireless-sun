using System;
using System.Collections.Generic;
using Godot;

public class GameHandler : Node2D {
	static public GameHandler Instance { get { return myInstance; } }

	static private GameHandler myInstance = null;
	static public Body myPlayer;
	static public Dictionary<string, uint> Layer = new Dictionary<string, uint> ();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		Global._OnGameSceneStarted ();
		myPlayer = GetNode<Body> ("PlayerBody");
		myInstance = this;

		// Building Layer List
		for (int i = 1; i <= 20; i += 1) {
			string name = (string) ProjectSettings.GetSetting ("layer_names/2d_physics/layer_" + i);
			if (name == "")
				name = "layer" + i;
			Layer[name] = (uint) 1 << i - 1;
		} // TODO move this to Global

		if (Network.IsConnectionStarted) {
			ControlMyPlayer (GetTree ().GetNetworkUniqueId ());
			Network._OnGameHandlerAwake ();
		}
	}

	public static void SpawnPlayer (int id, Vector2 position) {
		if (Instance == null)
			return;
		GD.Print ("New player entering : " + id);
		var newPlayer = ((PackedScene) GD.Load ("res://Nodes/Bodies/PlayerBody.tscn")).Instance ().GetNode<Body> ("./");
		newPlayer.Name = id.ToString ();
		newPlayer.SetNetworkMaster (id);
		newPlayer.Position = position;
		Instance.AddChild (newPlayer);
	}

	public static void RemovePlayer (int id) {
		if (Instance == null)
			return;
		Instance.GetNode<Node> (id.ToString ()).QueueFree ();
	}

	public void ControlMyPlayer (int id) {
		SetNetworkMaster (1);
		myPlayer.Name = id.ToString ();
		myPlayer.SetNetworkMaster (id);
	}

	public override void _Process (float delta) {
		if (!Network.IsConnectionStarted || IsNetworkMaster ()) {
			if (Input.IsActionPressed ("save"))
				Global.SaveGame ();
		}
	}
}
