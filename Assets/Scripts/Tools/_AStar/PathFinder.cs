using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AStar;

public class PathFinder {

	public MapData mapData;
	public Node[] nodeList;

	public PathFinder(MapData mapData) {
		this.mapData = mapData;
		InitNodeList();
	}


	void InitNodeList() {
		nodeList = new Node[mapData.mapWidth * mapData.mapHeight];
		for (int i = 0; i < mapData.tileTypes.Length; i++) {
			nodeList [i] = CreateNode (i);
		}
	}

	Node CreateNode(int index) {

		bool walkable = mapData.tileTypes[index] == 0;
		int gridX = index % mapData.mapWidth;
		int gridY = Mathf.FloorToInt(index / mapData.mapWidth);
		return new Node(walkable, gridX, gridY, index);
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (Mathf.Abs (x) == Mathf.Abs (y)) {
					continue;
				}

				if (IsInRange (checkX, checkY)) {
					int index = checkY * mapData.mapWidth + checkX;
//					Debug.Log (index);
					neighbours.Add (nodeList [index]);
				}
			}
		}

		return neighbours;
	}

    bool IsInRange(int x, int y) {
        return (x >= 0 && x < mapData.mapWidth && y >= 0 && y < mapData.mapHeight);
    }

	public List<int> FindPath(int startIndex, int targetIndex) {

		Node startNode = CreateNode(startIndex);
		Node targetNode = CreateNode(targetIndex);

		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node>(mapData.mapWidth * mapData.mapHeight);
			HashSet<Node> closeSet = new HashSet<Node>();
			openSet.Add(startNode);

			while(openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closeSet.Add(currentNode);

				if (currentNode.gridX == targetNode.gridX && currentNode.gridY == targetNode.gridY) {
					return GetPath(startNode, currentNode);
				}

				foreach (Node neighbour in GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closeSet.Contains(neighbour))
						continue;
					int newCoseToNieghbour = currentNode.gCost + GetDistance(currentNode, neighbour);

					if (newCoseToNieghbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newCoseToNieghbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						else
							openSet.UpdateItem(neighbour);
					}
				}
			}
		}

		return null;
	}

    List<int> GetPath(Node startNode, Node targetNode) {
        List<int> path = new List<int>();
        Node current = targetNode;

		while(current != null && !(current.gridX == startNode.gridX && current.gridY == startNode.gridY)) {
			path.Add(current.index);
            current = current.parent;
		}
        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		return disX + disY;
    }

	public List<int> GetMoveableList(int index, int moveAbility) {

		Node startNode = CreateNode (index);
		List<int> movableNodes = new List<int>();
		Queue<Node> checkQueue = new Queue<Node>();
		startNode.mCost = 0;
		movableNodes.Add(startNode.index);
		checkQueue.Enqueue(startNode);

		while(checkQueue.Count > 0) {
			Node currentNode = checkQueue.Dequeue();
			List<Node> neighbours = GetNeighbours(currentNode);
			foreach(Node neighbour in neighbours) {
				if (!neighbour.walkable || checkQueue.Contains(neighbour) || movableNodes.Contains(neighbour.index))
					continue;

				neighbour.mCost = currentNode.mCost + GetDistance(neighbour, currentNode);
				if (neighbour.mCost <= moveAbility) {
					checkQueue.Enqueue(neighbour);
					movableNodes.Add(neighbour.index);
				}
			}
		}

		return movableNodes;
	}

}