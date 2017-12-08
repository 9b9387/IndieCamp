using System;

namespace StateMachine {
	
	public interface IStatable<T> {

		IState<T> PreviousState {
			get;
			set;
		}

		IState<T> CurrentState {
			get;
			set;
		}

		void MakeTransition (IState<T> newState);
	}
}
