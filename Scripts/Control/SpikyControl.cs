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
				CurrentDirection = (DiagonalDirection) ((MyMovement.NextMovement.Angle () * 2 / Mathf.Pi + 4) % 4); //We should Optimize (and arrange it so it fits the isometric world)
			} else if (CurrentState == "idle") {
				if (CanSeePlayer && CanMove) {
					CurrentState = "run";
					return;
				}
			}
		}
	}
}
