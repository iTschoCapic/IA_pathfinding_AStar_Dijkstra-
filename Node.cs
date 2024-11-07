using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Renderer renderer = new Renderer();
    public TerrainType Type;
    public int numberOfNeighbours = 0;
    public List<Node> Neighbours = new List<Node>();
    public Vector2 position;
    public bool visited;

    public Node(TerrainType Type)
    {
        this.Type = Type;
    }

    public Node(TerrainType Type, List<Node> NextNode)
    {
        this.Type = Type;
        this.Neighbours = NextNode;
        this.numberOfNeighbours = this.Neighbours.Count;
    }

    public void SetNextNeighbours(Node Neighbour)
    {
        this.Neighbours.Add(Neighbour);
        numberOfNeighbours++;
    }

    public Node GetNextNeighbour(int delta)
    {
        return this.Neighbours[delta];
    }

    public List<Node> GetNeighbors()
    {
        return this.Neighbours;
    }

    public int GetTerrainCost()
    {
        switch (this.Type)
        {
            case TerrainType.Grass: return 1;
            case TerrainType.Mud: return 3;
            case TerrainType.Water: return 5;
            case TerrainType.Mountain: return 10;
            default: return 1;
        }
    }
}