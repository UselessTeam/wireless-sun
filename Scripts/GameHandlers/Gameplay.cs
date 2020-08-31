using System;
using System.Collections.Generic;
using Godot;

public class Gameplay : Node2D {
	static public Gameplay Instance { get { return myInstance; } }
	static private Gameplay myInstance = null;

	static public Inventory inventory;

	static private Camera2D myCamera;
	static private Position2D spawnPoint;

	static public KinematicPiece myPlayer;

	static public Dictionary<string, uint> Layer = new Dictionary<string, uint> ();

	[Signal] public delegate void PlayerRespawn ();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		myInstance = this;

		GameRoot.GameplayStart ();

		myCamera = ((PackedScene) GD.Load ("res://Nodes/Camera2D.tscn")).Instance ().GetNode<Camera2D> ("./");
		Instance.AddChild (myCamera);

		// Spawn player at the designated spawnpoint
		spawnPoint = GetNode<Position2D> ("SpawnPoint");

		GD.Print ("Gameple spawn");

		Respawn (GetNodeOrNull<KinematicPiece> ("MyPlayer"));

		// Building Layer List
		for (int i = 1; i <= 20; i += 1) {
			string name = (string) ProjectSettings.GetSetting ("layer_names/2d_physics/layer_" + i);
			if (name == "")
				name = "layer" + i;
			Layer[name] = (uint) 1 << i - 1;
		} // TODO move this to Global
		GameRoot.GameplayReady ();
	}

	public static KinematicPiece SpawnPlayer (int id, Vector2 position) {
		if (Instance == null)
			return null;;
		GD.Print ("New player entering : " + id);
		var newPlayer = ((PackedScene) GD.Load ("res://Nodes/Bodies/PlayerBody.tscn")).Instance ().GetNode<KinematicPiece> ("./");
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

	public override void _Input (InputEvent _input) {
		if (myPlayer == null && _input.IsActionPressed ("respawn"))
			Respawn ();
		if (_input.IsActionPressed ("pause")) {
			// if (!Network.IsConnectionStarted) {
			GetTree ().Paused = true;
			GetNode<Control> ("/root/GUI/PauseMenu").Show ();
			// }
		}
		if (_input.IsActionPressed ("inventory")) {
			GameRoot._GUI.Toggle (GUI.Window.Inventory);
		}
	}

	public override void _Process (float delta) {}

	public void _OnPlayerDied () {
		GetNode<CanvasItem> ("/root/GUI/DeathScreen").Show ();
		DetachCamera ();
		myPlayer.QueueFree ();
		myPlayer = null;
	}

	public void Respawn (KinematicPiece cameraHolder = null) {
		GetNode<CanvasItem> ("/root/GUI/DeathScreen").Hide ();
		int id = (Network.IsConnectionStarted) ? GetTree ().GetNetworkUniqueId () : 1;
		if (cameraHolder == null)
			cameraHolder = SpawnPlayer (id, spawnPoint.Position);
		else
			cameraHolder.Name = id.ToString ();
		myPlayer = cameraHolder;
		AttachCamera (myPlayer);
		EmitSignal (nameof (PlayerRespawn));
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
