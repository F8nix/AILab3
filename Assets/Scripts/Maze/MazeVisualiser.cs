using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MazeVisualizer : MonoBehaviour
{
  private readonly (int x, int y)[] directions = {
    (1, 0),
    (0, 1),
    (-1, 0),
    (0, -1)
  };
  public GameObject wallPrefab;
  public GameObject pathPrefab;
  public GameObject startPrefab;
  public GameObject targetPrefab;
  public MazeGenerator mazeGenerator;
  private const float wallTileSize = 0.34f;
  private List<GameObject> elements = new();

  private void Start()
  {
    mazeGenerator = GetComponent<MazeGenerator>();
    mazeGenerator.onMazeGenerated.AddListener(DrawMaze);
  }

  void DrawNodeWalls(Node node, int x, int y)
  {
    List<(int x, int y)> walls = new();
    foreach (var (dx, dy) in directions)
    {
      var (nx, ny) = (x + dx, y + dy);
      if (nx < 0 || nx >= mazeGenerator.width || ny < 0 || ny >= mazeGenerator.height) continue;
      var neighbor = mazeGenerator.graph[nx, ny];
      if (!node.neighbors.Contains(neighbor))
      {
        walls.Add((dx, dy));
        Debug.Log($"Node {x}, {y} has neighbor {dx}, {dy}");
      }
      if (node.x == 0)
      {
        walls.Add((-1, 0));
      }
      if (node.y == 0)
      {
        walls.Add((0, -1));
      }
      if (node.x == mazeGenerator.width - 1)
      {
        walls.Add((1, 0));
      }
      if (node.y == mazeGenerator.height - 1)
      {
        walls.Add((0, 1));
      }
    }

    foreach (var (dx, dy) in walls)
    {
      Vector3 offsetMult = Vector3.zero;
      Vector3 offsetBase = Vector3.zero;
      if (dx == 1)
      {
        offsetBase = new Vector3(1, -1, 0);
        offsetMult = new Vector3(0, 1, 0);
      }
      else if (dx == -1)
      {
        offsetBase = new Vector3(-1, -1, 0);
        offsetMult = new Vector3(0, 1, 0);
      }
      else if (dy == 1)
      {
        offsetBase = new Vector3(-1, 1, 0);
        offsetMult = new Vector3(1, 0, 0);
      }
      else if (dy == -1)
      {
        offsetBase = new Vector3(-1, -1, 0);
        offsetMult = new Vector3(1, 0, 0);
      }
      for (int i = 0; i <= 2; i++)
      {
        Vector3 pos = new Vector3(x, y, 0) + (i * wallTileSize * offsetMult) + (offsetBase * wallTileSize) + transform.position;
        elements.Add(Instantiate(wallPrefab, pos, Quaternion.identity));
      }
    }

  }
  void SpawnPath(Node node)
  {
    Vector3 pos = new Vector3(node.x, node.y, 0) + transform.position;
    elements.Add(Instantiate(pathPrefab, pos, Quaternion.identity));
  }
  void DrawMaze()
  {
    elements.ForEach(Destroy);
    elements.Clear();
    Graph graph = mazeGenerator.graph;

    foreach (var ((x, y), node) in graph.nodes)
    {
      SpawnPath(node);
      DrawNodeWalls(node, x, y);

      if (node == mazeGenerator.start)
      {
        Vector3 pos = new Vector3(node.x, node.y, 0) + transform.position;
        elements.Add(Instantiate(startPrefab, pos, Quaternion.identity));
        continue;
      }

      if (node == mazeGenerator.target)
      {
        Vector3 pos = new Vector3(node.x, node.y, 0) + transform.position;
        elements.Add(Instantiate(targetPrefab, pos, Quaternion.identity));
        continue;
      }

    }
  }
}