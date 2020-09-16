using System;
using Godot;

public class SpikyControl : MonsterControl {

	public override void _PhysicsProcess (float delta) {
		if (IsMaster) {
			if (CurrentState == "run") {
				if (!(CanSeePlayer && CanMove)) {
					CurrentState = "idle";
					return;
				}
				var playerPiece = MyFOV.GetClosestPlayer ();
				MyMovement.NextMovement = (playerPiece.GlobalPosition - MyPiece.GlobalPosition).Normalized ();
			} else if (CurrentState == "idle") {
				if (CanSeePlayer && CanMove) {
					CurrentState = "run";
					return;
				}
			}
		}
	}
}
