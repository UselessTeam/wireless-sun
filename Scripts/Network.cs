using System;
using Godot;

public class Network : Node2D
{
    // [Signal] delegate void PlayerDisconnected (int id);
    // [Signal] delegate void ServerDisconnected (int id);

    // const string DEFAULT_IP = "127.0.0.1";
    const string DEFAULT_IP = "192.168.1.5";
    const int DEFAULT_PORT = 31400;
    const int MAX_PLAYERS = 3;

    public static bool IsConnectionStarted = false;
    public static bool IsServer { get { return !IsConnectionStarted || Instance.GetTree().IsNetworkServer(); } }
    public static Network Instance { get { return instance; } }

    static Network instance;
    int serverID { get { return GetTree().GetNetworkUniqueId(); } }
    int nConnectedPlayers = 1;
    int[] connectedPlayers = new int[MAX_PLAYERS];

    GameHandler gameHandler { get { return GameHandler.Instance; } }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        instance = this;

        // Set network signals for the Network handler
        GetTree().Connect("network_peer_connected", this, "_OnPlayerConnected");
        GetTree().Connect("network_peer_disconnected", this, "_OnPlayerDisconnected");

        GetTree().Connect("connected_to_server", this, "_OnJoinedAServer");
        GetTree().Connect("server_disconnected", this, "_OnServerDisconnected");

        GetTree().Connect("connection_failed", this, "_OnConnectionFailed");
    }

    public void _OnPlayerDisconnected(int id)
    {
        GD.Print("Player leaving : " + id);
        gameHandler.RemovePlayer(id);

        for (int i = 0; i < nConnectedPlayers; i++)
        {
            if (connectedPlayers[i] == id)
            {
                nConnectedPlayers--;
                connectedPlayers[i] = connectedPlayers[nConnectedPlayers];
                connectedPlayers[nConnectedPlayers] = 0;
                return;
            }
        }
        GD.Print("ALERT! ID of the disconnecting player not found");
    }

    public void _OnPlayerConnected(int id)
    {
        connectedPlayers[nConnectedPlayers] = id;
        nConnectedPlayers++;

        gameHandler.RpcId(id, "SpawnPlayer", serverID, GameHandler.myPlayer.Position);

    }

    public void _OnJoinedAServer()
    {
        GD.Print("Server joined!");
        gameHandler.SetNetworkMaster(1);
        connectedPlayers[0] = serverID;
        gameHandler.ControlMyPlayer(serverID);
    }

    public void _OnServerDisconnected()
    {
        GD.Print("Disconnecting from server");
        GetTree().NetworkPeer = null;
        for (int i = 0; i < nConnectedPlayers; i++)
        {
            int id = connectedPlayers[i];
            if (id != 0 && connectedPlayers[i] != serverID)
                gameHandler.RemovePlayer(i);
        }

    }

    public void _OnConnectionFailed()
    {
        GD.Print("Connection failed !");
        IsConnectionStarted = false;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!IsConnectionStarted)
        {
            if (Input.IsActionJustPressed("host_game"))
            {
                Host();
            }
            if (Input.IsActionJustPressed("join_game"))
            {
                Join();
            }
        }
    }

    public void Host()
    {
        GD.Print("Creating server");
        var peer = new NetworkedMultiplayerENet();
        peer.CreateServer(DEFAULT_PORT, MAX_PLAYERS);
        GetTree().NetworkPeer = peer;
        IsConnectionStarted = true;
        _OnJoinedAServer();
    }
    public void Join()
    {
        GD.Print("Joining server");
        var peer = new NetworkedMultiplayerENet();
        peer.CreateClient(DEFAULT_IP, DEFAULT_PORT);
        GetTree().NetworkPeer = peer;
        IsConnectionStarted = true;
    }

}