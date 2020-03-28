using System;
using System.Collections.Generic;
using Godot;

public class Gameplay : Node2D {
	static public Gameplay Instance { get { return myInstance; } }
	static private Gameplay myInstance = null;

	static private Camera2D myCamera;
	static private Position2D spawnPoint;

	static public Body myPlayer;

	static public Dictionary<string, uint> Layer = new Dictionary<string, uint> ();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		GameRoot._OnGameSceneStarted ();
		myInstance = this;

		myCamera = ((PackedScene) GD.Load ("res://Nodes/Camera2D.tscn")).Instance ().GetNode<Camera2D> ("./");
		Instance.AddChild (myCamera);

		// Spawn player at the designated spawnpoint
		spawnPoint = GetNode<Position2D> ("SpawnPoint");
		Respawn ();

		// Building Layer List
		for (int i = 1; i <= 20; i += 1) {
			string name = (string) ProjectSettings.GetSetting ("layer_names/2d_physics/layer_" + i);
			if (name == "")
				name = "layer" + i;
			Layer[name] = (uint) 1 << i - 1;
		} // TODO move this to Global

		if (Network.IsConnectionStarted) {
			SetNetworkMaster (1);
			Network._OnGameHandlerAwake ();
		}
	}

	public static Body SpawnPlayer (int id, Vector2 position) {
		if (Instance == null)
			return null;;
		GD.Print ("New player entering : " + id);
		var newPlayer = ((PackedScene) GD.Load ("res://Nodes/Bodies/PlayerBody.tscn")).Instance ().GetNode<Body> ("./");
		newPlayer.Name = id.ToString ();
		newPlayer.SetNetworkMaster (id);
		newPlayer.Position = position;
		Instance.AddChild (newPlayer);
		return newPlayer;
	}

	public static void RemovePlayer (int id) {
		if (Instance == null)
			return;
		Instance.GetNode<Node> (id.ToString ()).QueueFree ();
	}

	public override void _Process (float delta) {
		if (!Network.IsConnectionStarted || IsNetworkMaster ()) {
			if (Input.IsActionPressed ("save"))
				Save.SaveGame ();
			if (myPlayer == null && Input.IsActionPressed ("respawn"))
				Respawn ();
		}
	}

	public void _OnPlayerDied () {
		GetNode<CanvasItem> ("/root/GUI/DeathScreen").Show ();
		DetachCamera ();
		myPlayer.QueueFree ();
		myPlayer = null;
	}

	public void Respawn () {
		GetNode<CanvasItem> ("/root/GUI/DeathScreen").Hide ();
		int id = (Network.IsConnectionStarted) ? GetTree ().GetNetworkUniqueId () : 1;
		myPlayer = SpawnPlayer (id, spawnPoint.Position);
		AttachCamera (myPlayer);
	}

	public void AttachCamera (Node node) {
		myCamera.Position = new Vector2 (0, 0);
		myCamera.GetParent ().RemoveChild (myCamera);
		node.AddChild (myCamera);
	}

	public void DetachCamera () {
		var pos = myCamera.GlobalPosition;
		myCamera.GetParent ().RemoveChild (myCamera);
		myCamera.GlobalPosition = pos;
		Instance.AddChild (myCamera);
	}

}