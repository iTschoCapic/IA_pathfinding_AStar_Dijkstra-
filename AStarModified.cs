using System.Collections.Generic;
using UnityEngine;

public class AStarModified : MonoBehaviour
{
    public Map map;
    public Node[,] graph;
    private List<Node> openSet = new List<Node>();
    private Dictionary<Node, float> gScore = new Dictionary<Node, float>();
    private Dictionary<Node, float> fScore = new Dictionary<Node, float>();

    public void Initialize(Node startNode)
    {
        map = FindObjectOfType<Main>().map;
        graph = map.node_map;

        foreach (Node node in graph)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }

        gScore[startNode] = 0;
        fScore[startNode] = GetHeuristic(startNode);

        openSet.Add(startNode);
    }

    public bool CaptureNextTile()
    {
        if (openSet.Count == 0)
            return false;

        Node currentNode = GetLowestFScoreNode(openSet, fScore);
        openSet.Remove(currentNode);

        // Capture the tile
        currentNode.Type = TerrainType.AStar;
        GameObject nodeObject = map.gameObjects[(int)currentNode.position.x, (int)currentNode.position.y];
        nodeObject.GetComponent<Renderer>().material.color = Color.red;

        // Add neighbors to openSet
        foreach (Node neighbor in currentNode.GetNeighbors())
        {

            if (neighbor.Type == TerrainType.Wall || neighbor.Type == TerrainType.AStar)
                continue;

            float tentativeGScore = gScore[currentNode] + GetDistance(currentNode, neighbor);
            if (tentativeGScore < gScore[neighbor])
            {
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + GetHeuristic(neighbor);
                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
            }
        }

        return true;
    }

    private Node GetLowestFScoreNode(List<Node> openSet, Dictionary<Node, float> fScore)
    {
        Node lowestNode = openSet[0];
        foreach (Node node in openSet)
        {
            if (fScore[node] < fScore[lowestNode])
                lowestNode = node;
        }
        return lowestNode;
    }

    private float GetDistance(Node a, Node b)
    {
        return Vector2.Distance(a.position, b.position);
    }

    private float GetHeuristic(Node node)
    {
        return Vector2.Distance(node.position, new Vector2(map.length[0] / 2, map.length[1] / 2));
    }
}
