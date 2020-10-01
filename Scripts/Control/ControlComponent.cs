using System;
using System.Collections.Generic;
using FSM;
using Godot;

public abstract class ControlComponent : Node2D {
	[Export] public float XpMultiplier = 1;

	public override void _Ready () {
		AddToGroup ("SaveNodes");
	}

	protected static Dictionary<string, State> StateList = new Dictionary<string, State> () { { "idle", new State ("idle") }, { "run", new State ("run") }
	};

	private State currentState = StateList["idle"];
	public State CurrentState {
		get { return currentState; }
		set {
			if (value != currentState) {
				currentState = value;
				SetAnimation ();
			}
		}
	}

	public bool CanMove { get { return MyMovement.CanMove; } }
	public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
	public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

	public MovementComponent MyMovement { get { return GetNodeOrNull<MovementComponent> ("../Movement"); } }
	public Node2D MyPiece { get { return GetParent<Node2D> (); } }
	public AnimatedSprite MySprite { get { return MyPiece.GetNode<AnimatedSprite> ("Display"); } }

	public void SetAnimation () {
		var dir = MyMovement.CurrentDirection;
		SetAnimation (dir);
	}

	public void SetAnimation (DirectionHelper.Direction dir) {
		string suffix = dir.ToString ();
		bool isRight = (int) dir < 3;
		if (this is PlayerControl)
			MySprite.FlipH = !isRight;
		else {
			MyPiece.Scale = new Vector2 ((isRight) ? -1 : 1, 1);
			GD.Print ("Scale ", MyPiece.Scale, " -- TRansform ", MyPiece.Transform);
		}
		if (isRight) suffix = suffix.Replace ("right", "left");
		MySprite.Play (CurrentState.Name + "_" + suffix);
	}

	[PuppetSync] public void _OnDied () {
		MyPiece.QueueFree ();
	}

	// Save and load data of the object in a file
	public Godot.Collections.Dictionary<string, object> MakeSave () {
		var saveObject = new Godot.Collections.Dictionary<string, object> () {
				{
					"Filename",
					MyPiece.Filename
				}, { "Name", MyPiece.Name }, {
					"Parent",
					MyPiece.GetParent ().GetPath ()
				}, { "ControlPosition", GetPositionInParent () }, {
					"PositionX",
					MyPiece.Position.x
				}, { "PositionY", MyPiece.Position.y }
			};
		return saveObject;
	}

	public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
		MyPiece.Position = new Vector2 (Convert.ToSingle (saveObject["PositionX"]), Convert.ToSingle (saveObject["PositionY"]));
	}
}
