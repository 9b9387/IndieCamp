namespace BehaviourTree{

	public class Sequence : Node {
		private Node[] m_nodes;
		private int m_curIdx;

		public Sequence(params Node[] nodes){
			m_nodes = nodes;
			m_curIdx = 0;
		}

		public override Result Execute (){
			for (; m_curIdx < m_nodes.Length;) {
				Result result = m_nodes [m_curIdx].Execute ();
				switch(result){
				case Result.success:
					m_curIdx++;
					continue;

				case Result.failure:
					m_curIdx = 0;
					return Result.failure;

				case Result.running:
					return Result.running;

				default:
					m_curIdx = 0;
					return Result.success;
				}
			}

			m_curIdx = 0;
			return Result.success;
		}
	}

}
