using UnityEngine;

public enum TerrainType
{
    Empty = 0,
    Wall = 1,
    AStar = 2,
    Dijkstra = 3,
    Bonus = 4,
    Grass = 5,
    Mud = 6,
    Water = 7,
    Mountain = 8
}

public class Map {

    public int[] length = new int[2] {0, 0};
    public Node[,] node_map;
    public GameObject[,] gameObjects;

    public Map(Node[,] node_map, GameObject[,] gameObjects, int[] length)
    {
        this.node_map = node_map;
        this.gameObjects = gameObjects;
        this.length = length;
    }
}
