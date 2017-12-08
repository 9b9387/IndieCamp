namespace BehaviourTree{

	using System;

	public class Action : Node {
		private Func<Result> m_action;

		public Action(Func<Result> action){
			m_action = action;
		}

		public override Result Execute (){
			return m_action.Invoke ();
		}
	}

}
