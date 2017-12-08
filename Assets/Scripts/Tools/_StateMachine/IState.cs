using System;

namespace StateMachine {
	
	public interface IState<T> { 

		T Owner {
			get;
			set;
		}

		void Enter (T owner);
		void Execute (); //call in update
		void Exit ();
	}
}
