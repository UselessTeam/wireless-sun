using System;
using Godot;

public class SpikyControl : MonsterControl {

	public override void _PhysicsProcess (float delta) {
		if (IsMaster) {
			switch (CurrentState.Name) {
				case "run":
					if (!(CanSeePlayer && CanMove)) {
						CurrentState = StateList["idle"];
						return;
					}
					var playerPiece = MyFOV.GetClosestPlayer ();
					MyMovement.NextMovement = (playerPiece.GlobalPosition - MyPiece.GlobalPosition).Normalized ();
					break;
				case "idle":
					if (CanSeePlayer && CanMove) {
						CurrentState = StateList["run"];
						return;
					}
					break;
			}
		}
	}
}
