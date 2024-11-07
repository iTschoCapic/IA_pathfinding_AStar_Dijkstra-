using System.IO;
using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    public Map map; // Toroidal map

    public GameObject[] prefabs;
    private StreamReader reader;
    private DijkstraModified pathfindingDijkstraModified;
    private AStarModified pathfindingAStarModified;
    private Dijkstra pathfindingDijkstra;
    private AStar pathfindingAStar;
    public int NumberOfTilesToCapture = 0;

    void Start()
    {
        map = new Map(null, null, new int[2] { 0, 0 });

        pathfindingDijkstraModified = new DijkstraModified();
        pathfindingAStarModified = new AStarModified();

        pathfindingDijkstra = new Dijkstra();
        pathfindingAStar = new AStar();

        reader = new StreamReader("./Assets/maps/default.txt");
        if (reader == null)
        {
            reader.Close();
        }
        decodeMap(reader);

        Node start = new Node(TerrainType.Empty);
        Node end = new Node(TerrainType.Empty);

        // Test for the modified version of AStar and Dijkstra

        pathfindingDijkstraModified.Initialize(map.node_map[11, 4]);
        pathfindingAStarModified.Initialize(map.node_map[0, 4]);

        StartCoroutine(AlternateCaptureTurns());

        // Test for the original version of AStar and Dijkstra

        //start = map.node_map[11, 4];
        //end = map.node_map[5, 1];
        //pathfindingDijkstra.FindPath(map, start, end);

        //start = map.node_map[0, 4];
        //end = map.node_map[7, 8];
        //pathfindingAStar.FindPath(start, end);
    }

    private IEnumerator AlternateCaptureTurns()
    {
        int aStarCaptured = 0;
        int dijkstraCaptured = 0;

        while (aStarCaptured < NumberOfTilesToCapture || dijkstraCaptured < NumberOfTilesToCapture)
        {
            // A* takes a turn
            if (aStarCaptured < NumberOfTilesToCapture)
            {
                bool aStarDidCapture = pathfindingAStarModified.CaptureNextTile();
                if (aStarDidCapture)
                {
                    aStarCaptured++;
                }
                yield return new WaitForSeconds(0.1f);
            }

            // Dijkstra takes a turn
            if (dijkstraCaptured < NumberOfTilesToCapture)
            {
                bool dijkstraDidCapture = pathfindingDijkstraModified.CaptureNextTile();
                if (dijkstraDidCapture)
                {
                    dijkstraCaptured++;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void Update()
    {
        // Allows the developper to check neighbors by clicking a tile.

        //if (Selection.activeGameObject != null)
        //{
        //    Node obj = map.node_map[(int)Selection.activeGameObject.transform.position.x, (int)Selection.activeGameObject.transform.position.z];

        //    for (int i = 0; i < obj.numberOfNeighbours; i++)
        //    {
        //        Node neighborNode = obj.GetNextNeighbour(i);

        //        int neighborX = -1, neighborZ = -1;
        //        for (int row = 0; row < map.length[0]; row++)
        //        {
        //            for (int col = 0; col < map.length[1]; col++)
        //            {
        //                if (map.node_map[row, col] == neighborNode)
        //                {
        //                    neighborX = row;
        //                    neighborZ = col;
        //                    break;
        //                }
        //            }
        //            if (neighborX != -1) break;
        //        }

        //        if (neighborX != -1 && neighborZ != -1)
        //        {
        //            GameObject neighborObject = map.gameObjects[neighborX, neighborZ];
        //            neighborObject.GetComponent<Renderer>().material.color = Color.green;
        //        }
        //    }
        //}
    }

    void decodeMap(StreamReader reader)
    {
        int i = 0;
        int j = 0;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] parts = line.Split(',');
            if (parts.Length != 2)
            {
                foreach (string part in parts)
                {
                    if (part.Contains("Wall"))
                    {
                        map.node_map[i, j] = new Node(TerrainType.Wall);
                        map.node_map[i, j].position = new Vector2(i, j);
                        map.gameObjects[i, j] = Instantiate(prefabs[1], new Vector3(i, 0, j), prefabs[1].transform.rotation);
                        map.gameObjects[i, j].GetComponent<Renderer>().material.color = Color.black;
                    }
                    else if (part.Contains("AStar"))
                    {
                        map.node_map[i, j] = new Node(TerrainType.AStar);
                        map.node_map[i, j].position = new Vector2(i, j);
                        map.gameObjects[i, j] = Instantiate(prefabs[2], new Vector3(i, 0, j), prefabs[2].transform.rotation);
                        map.gameObjects[i, j].GetComponent<Renderer>().material.color = Color.red;
                    }
                    else if (part.Contains("Djikstra"))
                    {
                        map.node_map[i, j] = new Node(TerrainType.Dijkstra);
                        map.node_map[i, j].position = new Vector2(i, j);
                        map.gameObjects[i, j] = Instantiate(prefabs[3], new Vector3(i, 0, j), prefabs[3].transform.rotation);
                        map.gameObjects[i, j].GetComponent<Renderer>().material.color = Color.blue;
                    }
                    else if (part.Contains("Bonus"))
                    {
                        map.node_map[i, j] = new Node(TerrainType.Bonus);
                        map.node_map[i, j].position = new Vector2(i, j);
                        map.gameObjects[i, j] = Instantiate(prefabs[4], new Vector3(i, 0, j), prefabs[4].transform.rotation);
                        map.gameObjects[i, j].GetComponent<Renderer>().material.color = Color.yellow;
                    }
                    else
                    {
                        map.node_map[i, j] = new Node(TerrainType.Empty);
                        map.node_map[i, j].position = new Vector2(i, j);
                        map.gameObjects[i, j] = Instantiate(prefabs[0], new Vector3(i, 0, j), prefabs[0].transform.rotation);
                        map.gameObjects[i, j].GetComponent<Renderer>().material.color = Color.white;
                    }
                    i++;
                }
                i = 0;
                j++;
            }
            else
            {
                map.length[0] = int.Parse(parts[0]);
                map.length[1] = int.Parse(parts[1]);
                map.node_map = new Node[int.Parse(parts[0]), int.Parse(parts[1])];
                map.gameObjects = new GameObject[int.Parse(parts[0]), int.Parse(parts[1])];
            }
        }

        for (int k = 0; k < map.length[0]; k++) // Iterate over x
        {
            for (int l = 0; l < map.length[1]; l++) // Iterate over z
            {
                Node currentNode = map.node_map[k, l];

                // Check each of the four possible neighbors (left, right, up, down)
                int[,] directions = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

                for (int d = 0; d < directions.GetLength(0); d++)
                {
                    // Calculate potential neighbor position
                    int neighborX = k + directions[d, 0];
                    int neighborZ = l + directions[d, 1];

                    // Apply wraparound for edges
                    if (neighborX < 0)
                    {
                        neighborX = map.length[0] - 1;
                    }
                    else if (neighborX >= map.length[0])
                    {
                        neighborX = 0;
                    }

                    if (neighborZ < 0)
                    {
                        neighborZ = map.length[1] - 1;
                    }
                    else if (neighborZ >= map.length[1])
                    {
                        neighborZ = 0;
                    }

                    Node neighborNode = map.node_map[neighborX, neighborZ];

                    // Skip if the neighbor node is a wall
                    if (neighborNode.Type == TerrainType.Wall)
                    {
                        continue;
                    }

                    // Set this neighbor as an adjacent node for pathfinding
                    currentNode.SetNextNeighbours(neighborNode);
                }
            }
        }

    }
}
