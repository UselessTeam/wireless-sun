using System;
using Godot;

public class Picker : Node2D {
    [Export] public float ITEM_SPEED;

    FieldOfView MyFOV { get { return GetNode<FieldOfView> ("FieldOfView"); } }

    public override void _Process (float delta) {
        if (Network.IsServer)
            foreach (Node2D item in MyFOV.GetOverlappingAreas ()) {
                item.GetNode<PickableControl> ("Control").Gather (GetNode<ControlComponent> ("../Control"));
            }
    }

}