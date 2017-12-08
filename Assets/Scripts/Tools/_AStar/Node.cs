namespace AStar{

	using UnityEngine;

	[System.Serializable]
	public class Node : IHeapItem<Node> {

		public int index;
	    public int gridX;
	    public int gridY;
		public Node parent;
		public bool walkable;

	    public int gCost;
	    public int hCost;
	    public int fCost {
	        get {
	            return gCost + hCost;
	        }
	    }

	    public int mCost; // 用于存储移动消耗

	    int heapIndex;
	    public int HeapIndex {
	        get {
	            return heapIndex;
	        }
	        set {
	            heapIndex = value;
	        }
	    }

		public Node(bool _walkable, int _gridX, int _gridY, int index) {
	        walkable = _walkable;
	        gridX = _gridX;
	        gridY = _gridY;
			this.index = index;
	    }

	    public int CompareTo(Node other) {
	        int result = fCost.CompareTo(other.fCost);
	        if (result == 0)
	            result = hCost.CompareTo(other.hCost);

	        return result;
	    }

		public bool IsEqual(Node another){
			bool result = this.walkable == another.walkable;
			result = result && this.gridX == another.gridX;
			result = result && this.gridY == another.gridY;

			return result;
		}
	}

}
