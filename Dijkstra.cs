using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public Map map;
    public Node[,] graph; // Your graph represented by a 2D array of nodes

    public void FindPath(Map map, Node startNode, Node endNode)
    {
        // Initialize distances and visited sets
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> Q = new List<Node>();

        // Initialize all distances to infinity and previous nodes to null
        foreach (var node in map.node_map)
        {
            if (node != null)
            {
                dist[node] = float.MaxValue;
                prev[node] = null;
                Q.Add(node);
            }
        }
        dist[startNode] = 0;

        while (Q.Count > 0)
        {
            // Get node with the minimum distance
            Node u = GetNodeWithMinDistance(Q, dist);
            if (u == null || u == endNode)
                break;

            Q.Remove(u);

            // Update distances for each neighbor of u
            foreach (var neighbor in u.GetNeighbors())
            {
                if (neighbor != null && Q.Contains(neighbor))
                {
                    float alt = dist[u] + neighbor.GetTerrainCost(); // GetTerrainCost() is here but not really used because I didn't add any special terrain.
                    if (alt < dist[neighbor])
                    {
                        dist[neighbor] = alt;
                        prev[neighbor] = u;
                    }
                }
            }
        }

        // Reconstruct the path from start to endNode
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != null)
        {
            currentNode = prev[currentNode];
            path.Add(currentNode);
        }

        path.Reverse(); // Reverse to get path from start to end

        // If the path is invalid (only endNode and no path to it), clear it
        if (path.Count == 1 && path[0] == endNode && prev[endNode] == null)
        {
            path.Clear();
            return;
        }

        foreach (Node node in path)  // For visual representation
        {
            GameObject nodeObject = map.gameObjects[(int)node.position.x, (int)node.position.y];
            nodeObject.GetComponent<Renderer>().material.color = Color.magenta;
        }
    }

    private Node GetNodeWithMinDistance(List<Node> Q, Dictionary<Node, float> dist)
    {
        Node minNode = null;
        float minDist = float.MaxValue;

        foreach (var node in Q)
        {
            if (dist[node] < minDist)
            {
                minDist = dist[node];
                minNode = node;
            }
        }

        return minNode;
    }
}