using System;
using System.Collections.Generic;
using Godot;

public class Beach : Node2D {
    static public Body PlayerBody;
    static public Dictionary<string, uint> Layer = new Dictionary<string, uint> ();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready () {
        PlayerBody = GetNode<Body> ("PlayerBody");

        // Building Layer List
        for (int i = 1; i <= 20; i += 1) {
            string name = (string) ProjectSettings.GetSetting ("layer_names/2d_physics/layer_" + i);

            if (name == "")
                name = "layer" + i;
            Layer[name] = (uint) 1 << i - 1;
        }

    }
}