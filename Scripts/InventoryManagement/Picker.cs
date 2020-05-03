using System;
using Godot;

public class Picker : Node2D {
    [Export] public float ITEM_SPEED;

    FieldOfView MyFOV { get { return GetNode<FieldOfView> ("FieldOfView"); } }

    public override void _Process (float delta) {
        if (Network.IsServer)
            foreach (Node2D item in MyFOV.GetOverlappingBodies ()) {
                MovementComponent itemPiece = item.GetNode<MovementComponent> ("Movement");
                itemPiece.NextMovement = (GetParent<KinematicPiece> ().GlobalPosition - itemPiece.GlobalPosition).Normalized ();
            }
    }

    public void AddToInventory () {
        //TODO
    }
}