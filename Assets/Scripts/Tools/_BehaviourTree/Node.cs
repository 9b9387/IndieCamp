namespace BehaviourTree{

	public enum Result{
		success,
		failure,
		running,
	}

	public abstract class Node {
		public abstract Result Execute ();
	}

}
