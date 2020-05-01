using System;
using Godot;

public struct PlayerData {
	public int id;
	public string name;
	public Vector2 position;
}

public class Network : Node2D {
	const string DEFAULT_IP = "127.0.0.1";
	// const string DEFAULT_IP = "192.168.1.5";
	const int DEFAULT_PORT = 31400;
	const int MAX_PLAYERS = 4;

	public static bool IsConnectionStarted = false;
	public static bool IsServer { get { return !IsConnectionStarted || Instance.GetTree ().IsNetworkServer (); } }
	public static Network Instance { get { return instance; } }

	static Network instance;
	int serverID { get { return GetTree ().GetNetworkUniqueId (); } }
	int nConnectedPlayers = 1;
	int[] connectedPlayers = new int[MAX_PLAYERS];

	// GameHandler gameHandler { get { return GameHandler.Instance; } }

	void PrintPlayers () {
		GD.Print ("Players:");
		for (int i = 0; i < nConnectedPlayers; i++)
			GD.Print (connectedPlayers[i].ToString ());
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		instance = this;

		// Set network signals for the Network handler
		GetTree ().Connect ("network_peer_connected", this, "_OnPlayerConnected");
		GetTree ().Connect ("network_peer_disconnected", this, "_OnPlayerDisconnected");

		GetTree ().Connect ("connected_to_server", this, "_OnJoinedAServer");
		GetTree ().Connect ("server_disconnected", this, "_OnServerDisconnected");

		GetTree ().Connect ("connection_failed", this, "_OnConnectionFailed");
	}

	public static void Host () {
		GD.Print ("Creating server");
		var peer = new NetworkedMultiplayerENet ();
		peer.CreateServer (DEFAULT_PORT, MAX_PLAYERS);
		Instance.GetTree ().NetworkPeer = peer;
		IsConnectionStarted = true;
		Instance._OnJoinedAServer ();
	}

	public static void Join (string IP) {
		GD.Print ("Joining server");
		var peer = new NetworkedMultiplayerENet ();
		peer.CreateClient ((IP != "") ? IP : DEFAULT_IP, DEFAULT_PORT);
		Instance.GetTree ().NetworkPeer = peer;
		IsConnectionStarted = true;
		GameRoot.Instance.Connect (nameof (GameRoot.GameplayStarted), instance, nameof (_OnGameplayStarted));
	}

	public void _OnGameplayStarted () {
		for (int i = 1; i < Instance.nConnectedPlayers; i++)
			Gameplay.SpawnPlayer (Instance.connectedPlayers[i], new Vector2 (0, 0));
		Gameplay.Instance.SetNetworkMaster (1);
	}

	public void _OnPlayerConnected (int id) {
		connectedPlayers[nConnectedPlayers] = id;
		nConnectedPlayers++;
		PrintPlayers ();

		Gameplay.SpawnPlayer (id, new Vector2 (0, 0));
		// gameHandler.RpcId (id, "SpawnPlayer ", serverID, GameHandler.myPlayer.Position);

	}

	public void _OnPlayerDisconnected (int id) {
		GD.Print ("Player leaving: " + id);
		// gameHandler.RemovePlayer (id);

		for (int i = 0; i < nConnectedPlayers; i++) {
			if (connectedPlayers[i] == id) {
				nConnectedPlayers--;
				connectedPlayers[i] = connectedPlayers[nConnectedPlayers];
				connectedPlayers[nConnectedPlayers] = 0;
				Gameplay.RemovePlayer (connectedPlayers[i]);
				return;
			}
		}
		GD.Print ("ALERT!ID of the disconnecting player not found ");
	}

	public void _OnJoinedAServer () {
		GD.Print ("Server joined!");
		connectedPlayers[0] = serverID;
		// gameHandler.ControlMyPlayer (serverID);
		PrintPlayers ();
	}

	public void DisconnectNetwork () {
		IsConnectionStarted = false;
		GetTree ().NetworkPeer = null;
	}

	public void _OnServerDisconnected () {
		GD.Print ("Lost connection with the server");
		DisconnectNetwork ();
		GameRoot.LoadMenuScene ();
	}

	public void _OnConnectionFailed () {
		GD.Print ("Connection failed!");
		IsConnectionStarted = false;
	}
}