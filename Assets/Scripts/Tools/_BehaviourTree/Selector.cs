namespace BehaviourTree{
	using System;
	using UnityEngine;

	public class Selector : Node {
		private Node[] m_nodes;
		private int m_curIdx = 0;
		private bool m_isRandomSelector = false;
		private int m_startIdx = -1;

		public Selector(params Node[] nodes){
			m_nodes = nodes;
			m_curIdx = 0;
			m_isRandomSelector = false;
			m_startIdx = -1;
		}

		public Selector(bool isRandomSeletor, params Node[] nodes){
			m_nodes = nodes;
			m_isRandomSelector = isRandomSeletor;
			m_curIdx = 0;
			if (m_isRandomSelector) {
				System.Random random = new System.Random (DateTime.Now.Millisecond);
				m_curIdx = random.Next (0, m_nodes.Length - 1);
			}
			m_startIdx = -1;
		}

		public Selector(int startIdx, params Node[] nodes){
			m_nodes = nodes;
			m_isRandomSelector = false;
			m_curIdx = startIdx;
			m_startIdx = startIdx;
			if (m_curIdx >= m_nodes.Length || m_curIdx < 0){
				Debug.LogWarning ("startIdx out of range, use 0 instead!");
				m_curIdx = 0;
				m_startIdx = -1;
			}
		}

		public override Result Execute (){
			for (; m_curIdx < m_nodes.Length;) {
				Result result = m_nodes [m_curIdx].Execute ();
				switch (result){
				case Result.failure:
					m_curIdx++;
					continue;

				case Result.success:
					m_curIdx = 0;
					if (m_isRandomSelector){
						System.Random random = new System.Random (DateTime.Now.Millisecond);
						m_curIdx = random.Next (0, m_nodes.Length - 1);
					}

					if (m_startIdx != -1){
						m_curIdx = m_startIdx;
					}

					return Result.success;
				
				case Result.running:
					return Result.running;

				default:
					m_curIdx++;
					continue;
				}
			}

			m_curIdx = 0;
			if (m_isRandomSelector){
				System.Random random = new System.Random (DateTime.Now.Millisecond);
				m_curIdx = random.Next (0, m_nodes.Length - 1);
			}

			if (m_startIdx != -1){
				m_curIdx = m_startIdx;
			}

			return Result.failure;
		}
	}

}
