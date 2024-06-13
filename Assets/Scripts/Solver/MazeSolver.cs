using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MazeSolver : MonoBehaviour
{
  public MazeGenerator mazeGenerator;
  private Graph graph;
  public DistanceType distanceType;
  private Dictionary<Node, int> estimatedDistance;
  private Dictionary<Node, AStarNode> nodes;

  public UnityEvent<List<Node>> onPathFound;

  public enum DistanceType
  {
    FROM_TARGET,
    FROM_START,
    COMBINED
  }
  public int Heuristic(AStarNode current)
  {
    // Using manhattan distance as the base because of a square grid.
    if (distanceType == DistanceType.FROM_TARGET)
    {
      //H
      return current.distance + Mathf.Abs(current.node.x - mazeGenerator.target.x) + Mathf.Abs(current.node.y - mazeGenerator.target.y);
    }
    else if (distanceType == DistanceType.FROM_START)
    {
      // G
      return current.distance + 1;
    }
    else if (distanceType == DistanceType.COMBINED)
    {
      // F - H + G
      return Mathf.Abs(current.node.x - mazeGenerator.target.x) + Mathf.Abs(current.node.y - mazeGenerator.target.y) + current.distance + 1;
    }
    return 0;
  }

  /// Returning IEnumerator for nice visualisation of which node is currently being checked
  public IEnumerator<Node> AStar()
  {
    Initialize();
    PriorityQueue<AStarNode> queue = new();
    queue.Enqueue(0, nodes[mazeGenerator.start]);
    List<Node> steps = new();
    do
    {
      var current = queue.Dequeue();
      if (current.node == mazeGenerator.target)
      {

        steps.Add(current.node);
        yield return current.node;
        List<Node> path = new()
        {
          current.node
        };
        while (current.previous != null)
        {
          path.Add(current.previous.node);
          current = current.previous;
        }
        path.Reverse();
        onPathFound?.Invoke(path);
        yield break;
      }
      steps.Add(current.node);
      CalculateNode(current, queue, steps);
      yield return current.node;
    } while (!queue.IsEmpty());

    yield break;
  }
  public void CalculateNode(AStarNode node, PriorityQueue<AStarNode> queue, List<Node> steps)
  {
    foreach (Node next in node.node.neighbors)
    {
      var nextNode = nodes[next];
      var distance = Heuristic(node);
      if (steps.Contains(next))
      {
        if (distance >= nextNode.distance)
          continue;
        steps.Remove(node.node);

      }
      if (queue.Contains(nextNode))
      {
        if (distance >= nextNode.distance)
          continue;
      }
      nextNode.distance = distance;
      nextNode.previous = node;
      nextNode.estimate = distance + Heuristic(nextNode);
      if (!queue.Contains(nextNode))
        queue.Enqueue(distance, nextNode);
    }
  }
  public void Initialize()
  {
    graph = mazeGenerator.graph;
    estimatedDistance = new();
    nodes = new();
    foreach (var (_, node) in graph.nodes)
    {
      nodes.Add(node, new AStarNode(node));
    }
    var startingNode = nodes[mazeGenerator.start];
    startingNode.distance = 0;
    startingNode.estimate = Heuristic(startingNode);
    var targetNode = nodes[mazeGenerator.target];
    targetNode.estimate = 0;
  }
}

public class AStarNode
{

  public Node node;
  public AStarNode previous = null;
  public int distance = int.MaxValue;
  public int estimate = int.MaxValue;
  public AStarNode(Node node)
  {
    this.node = node;
  }

}