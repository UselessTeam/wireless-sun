using System;
using System.Collections.Generic;
using Godot;

public class GameHandler : Node2D {
    static public GameHandler Instance { get { return myInstance; } }

    static private GameHandler myInstance;
    static public Body myPlayer;
    static public Dictionary<string, uint> Layer = new Dictionary<string, uint> ();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {
        myPlayer = GetNode<Body> ("PlayerBody");
        myInstance = this;

        // Building Layer List
        for (int i = 1; i <= 20; i += 1) {
            string name = (string) ProjectSettings.GetSetting ("layer_names/2d_physics/layer_" + i);

            if (name == "")
                name = "layer" + i;
            Layer[name] = (uint) 1 << i - 1;
        }
    }

    [Remote]
    public void SpawnPlayer (int id, Vector2 position) {
        GD.Print ("New player entering : " + id);
        var newPlayer = ((PackedScene) GD.Load ("res://Nodes/Bodies/PlayerBody.tscn")).Instance ().GetNode<Body> ("./");
        newPlayer.Name = id.ToString ();
        newPlayer.SetNetworkMaster (id);
        newPlayer.Position = position;
        AddChild (newPlayer);

    }

    public void ControlMyPlayer (int id) {
        myPlayer.Name = id.ToString ();
        myPlayer.SetNetworkMaster (id);
    }

    public void RemovePlayer (int id) {
        GetNode<Node> (id.ToString ()).QueueFree ();
    }

    public void GoToMenu () {
        //Go TO Main Menu
    }
}