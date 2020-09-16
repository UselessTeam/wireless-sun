using System.Collections.Generic;

namespace FSM {
	public class State {
		public string Name;
		public string NextState = null;
		public float TimeToNext = 0f;
		public bool CanMove = false;
		public string Animation {
			get {
				return Name.Split ('_') [0];
			}
		}

		public State (string name, string nextState = null, float timeToNext = 0f, bool canMove = false) {
			Name = name;
			NextState = nextState;
			TimeToNext = timeToNext;
			CanMove = canMove;
		}
	}

	public class StateManager {
		private List<State> states = new List<State> ();
		private int currentState = -1;
		public State CurrentState { get { return (currentState > -1) ? states[currentState] : null; } }

		public bool SetState (string stateName) { return false; }
	}

	public static class StateExtensions {
		public static Dictionary<string, State> IntoDictionnary (this List<State> states) {
			Dictionary<string, State> dictStates = new Dictionary<string, State> ();
			foreach (var state in states) {
				dictStates.Add (state.Name, state);
			}
			return dictStates;
		}
	}
}
