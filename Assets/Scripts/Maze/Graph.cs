using System.Collections.Generic;
using UnityEngine;

public class Node
{
  public int x, y;
  public List<Node> neighbors;

  public Node(int x, int y)
  {
    this.x = x;
    this.y = y;
    neighbors = new List<Node>();
  }
  public Node GetRandomNeighbor()
  {
    return neighbors[Random.Range(0, neighbors.Count)];
  }
}

public class Graph
{
  public int width, height;
  public Dictionary<(int x, int y), Node> nodes;

  public Graph(int width, int height)
  {
    this.width = width;
    this.height = height;
    nodes = new();

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        var node = new Node(x, y);
        nodes.Add((x, y), node);
      }
    }
  }

  public Node GetNode(int x, int y)
  {
    return this[x, y];
  }
  public Node this[int x, int y]
  {
    get => nodes.GetValueOrDefault((x, y), null);
    set
    {
      nodes[(x, y)] = value;
    }
  }

  public List<Node> GetNeighbors(Node node)
  {
    List<Node> neighbors = new();

    if (node.x > 0) neighbors.Add(nodes[(node.x - 1, node.y)]);
    if (node.x < width - 1) neighbors.Add(nodes[(node.x + 1, node.y)]);
    if (node.y > 0) neighbors.Add(nodes[(node.x, node.y - 1)]);
    if (node.y < height - 1) neighbors.Add(nodes[(node.x, node.y + 1)]);

    return neighbors;
  }
  public Node GetRandomNode()
  {
    var randomX = Random.Range(0, width);
    var randomY = Random.Range(0, height);

    return nodes[(randomX, randomY)];
  }

  public void AddEdge(Node a, Node b)
  {
    a.neighbors.Add(b);
    b.neighbors.Add(a);
  }
}