using System;
using Godot;

public class Picker : Node2D {
    [Export] public static float ITEM_SPEED;

    FieldOfView MyFOV { get { return GetNode<FieldOfView> ("FieldOfView"); } }

    public override void _Process (float delta) {
        if (Network.IsServer)
            foreach (var item in MyFOV.GetOverlappingBodies ()) {
                Body itemBody = (Body) item;
                itemBody.NextMovement = (GetParent<Body> ().Position - itemBody.Position).Normalized ();
            }
    }

    public void AddToInventory () {
        //TODO
    }
}