using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public Map map;
    public Node[,] graph;

    public void FindPath(Node startNode, Node endNode)
    {
        // Dictionaries to track costs and path and the previous Node
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        Main mainComponent = FindObjectOfType<Main>();
        map = mainComponent.map;
        graph = map.node_map;

        // Initialize gScore and fScore for all nodes
        foreach (Node node in graph)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }

        gScore[startNode] = 0; // Cost from start to start is zero
        fScore[startNode] = GetHeuristic(startNode, endNode); // Heuristic from start to goal

        // OpenSet to track nodes to explore
        List<Node> openSet = new List<Node> { startNode };

        while (openSet.Count > 0)
        {
            Node currentNode = GetLowestFScoreNode(openSet, fScore);

            if (currentNode == endNode) // Path found
            {
                ReconstructPath(cameFrom, currentNode);
                return;
            }

            openSet.Remove(currentNode);

            foreach (Node neighbor in currentNode.GetNeighbors())
            {
                if (neighbor.Type == TerrainType.Wall) // Ignore walls
                    continue;

                float tentativeGScore = gScore[currentNode] + GetDistance(currentNode, neighbor);

                if (tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + GetHeuristic(neighbor, endNode);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        Debug.Log("No path found");
    }

    private Node GetLowestFScoreNode(List<Node> openSet, Dictionary<Node, float> fScore)
    {
        Node lowestNode = openSet[0];

        foreach (Node node in openSet)
        {
            if (fScore[node] < fScore[lowestNode])
            {
                lowestNode = node;
            }
        }

        return lowestNode;
    }

    // Technically this two functions are the same but to not get lost when we want the distance and the heuristic we go with two functions.
    private float GetDistance(Node a, Node b)
    {
        return Vector2.Distance(a.position, b.position);
    }

    private float GetHeuristic(Node a, Node b)
    {
        return Vector2.Distance(a.position, b.position);
    }

    private void ReconstructPath(Dictionary<Node, Node> cameFrom, Node currentNode)
    {
        List<Node> totalPath = new List<Node> { currentNode };
        while (cameFrom.ContainsKey(currentNode))
        {
            currentNode = cameFrom[currentNode];
            totalPath.Add(currentNode);
        }

        totalPath.Reverse(); // Reverse to get path from start to end

        foreach (Node node in totalPath) // For visual representation
        {
            GameObject nodeObject = map.gameObjects[(int)node.position.x, (int)node.position.y];
            nodeObject.GetComponent<Renderer>().material.color = Color.cyan;
        }
    }
}
