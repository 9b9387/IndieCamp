namespace BehaviourTree{

	using System;

	public class Condition : Node {
		private Func<bool> m_test;

		public Condition(Func<bool> test){
			m_test = test;
		}

		public override Result Execute (){
			bool result = m_test.Invoke ();
			if (result){
				return Result.success;
			}

			return Result.failure;
		}
	}

}
