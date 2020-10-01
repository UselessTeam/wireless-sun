using System.Collections.Generic;

namespace FSM {
	public class State {
		public string Name;
		public string NextState = null;
		public float TimeToNext = 0f;
		public bool CanMove = true;
		public string Animation {
			get {
				return Name.Split ('_') [0];
			}
		}

		public State (string name, string nextState = null, float timeToNext = 0f, bool canMove = true) {
			Name = name;
			NextState = nextState;
			TimeToNext = timeToNext;
			CanMove = canMove;
		}
	}

	// public class StateManager {
	// 	private Dictionary<string, State> states = new Dictionary<string, State> ();
	// 	private string currentState = null;
	// 	public State CurrentState { get { return (currentState != null) ? states[currentState] : null; } }

	// 	private float stateTimer = 0f;

	// 	public bool SetState (string stateName) {
	// 		if (states.ContainsKey (currentState)) {
	// 			currentState = stateName;
	// 			if (CurrentState.NextState != null)
	// 				stateTimer = CurrentState.TimeToNext;
	// 			return true;
	// 		}
	// 		return false;
	// 	}

	// 	public void Update (float delta) {
	// 		if (stateTimer > 0) {
	// 			stateTimer -= delta;
	// 			if (stateTimer <= 0)
	// 				SetState (CurrentState.NextState);
	// 		}
	// 	}
	// }
}
