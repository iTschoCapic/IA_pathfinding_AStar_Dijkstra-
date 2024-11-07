using System.Collections.Generic;
using UnityEngine;

public class DijkstraModified : MonoBehaviour
{
    public Map map;
    public Node[,] graph;
    private List<Node> openSet = new List<Node>();
    private Dictionary<Node, float> costs = new Dictionary<Node, float>();

    public void Initialize(Node startNode)
    {
        map = FindObjectOfType<Main>().map;
        graph = map.node_map;

        foreach (Node node in graph)
        {
            costs[node] = float.MaxValue;
        }

        costs[startNode] = 0;
        openSet.Add(startNode);
    }

    public bool CaptureNextTile()
    {
        if (openSet.Count == 0)
            return false;

        Node currentNode = GetLowestCostNode(openSet, costs);
        openSet.Remove(currentNode);

        // Capture the tile
        currentNode.Type = TerrainType.Dijkstra;
        GameObject nodeObject = map.gameObjects[(int)currentNode.position.x, (int)currentNode.position.y];
        nodeObject.GetComponent<Renderer>().material.color = Color.blue;

        // Add neighbors to openSet
        foreach (Node neighbor in currentNode.GetNeighbors())
        {

            if (neighbor.Type == TerrainType.Wall || neighbor.Type == TerrainType.Dijkstra)
                continue;
            

            float newCost = costs[currentNode] + GetDistance(currentNode, neighbor);
            if (newCost < costs[neighbor])
            {
                costs[neighbor] = newCost;
                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
            }
        }

        return true;
    }

    private Node GetLowestCostNode(List<Node> openSet, Dictionary<Node, float> costs)
    {
        Node lowestNode = openSet[0];
        foreach (Node node in openSet)
        {
            if (costs[node] < costs[lowestNode])
                lowestNode = node;
        }
        return lowestNode;
    }

    private float GetDistance(Node a, Node b)
    {
        return Vector2.Distance(a.position, b.position);
    }
}
