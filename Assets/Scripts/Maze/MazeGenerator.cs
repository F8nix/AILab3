using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class MazeGenerator : MonoBehaviour
{
  public int width = 10;
  public int height = 10;
  public Graph graph;
  public UnityEvent onMazeGenerated;
  public Node start;
  public Node target;
  private void Start()
  {
    StartCoroutine(GenerateMaze());
  }

  IEnumerator GenerateMaze()
  {
    yield return new WaitForEndOfFrame();

    graph = new Graph(width, height);
    start = graph.GetRandomNode();
    target = graph.GetRandomNode();
    while (start == target)
    {
      target = graph.GetRandomNode();
    }
    var visitedStack = new Stack<Node>();
    var visitedSet = new HashSet<Node>();
    visitedSet.Add(start);
    visitedStack.Push(start);
    while (visitedStack.Count > 0)
    {
      var currentNode = visitedStack.Pop();
      var unvisitedNeighbors = GetUnvisitedNeighbors(currentNode, visitedSet);
      if (unvisitedNeighbors.Count > 0)
      {
        var randomIndex = Random.Range(0, unvisitedNeighbors.Count);
        var randomNeighbor = unvisitedNeighbors[randomIndex];

        graph.AddEdge(currentNode, randomNeighbor);
        visitedStack.Push(currentNode);
        visitedStack.Push(randomNeighbor);
        visitedSet.Add(randomNeighbor);
      }
    }


    onMazeGenerated?.Invoke();
  }
  List<Node> GetUnvisitedNeighbors(Node node, HashSet<Node> visited) => graph.GetNeighbors(node).Where(x => !visited.Contains(x)).ToList();

  List<Node> GetNeighbors(Node node) => graph.GetNeighbors(node);

  public void OnDrawGizmosSelected()
  {
    if (!Application.isPlaying) return;
    if (graph == null) return;
    Gizmos.color = Color.blue;
    foreach (var ((x, y), node) in graph.nodes)
    {
      if (node == start)
      {
        Gizmos.color = Color.green;
      }
      else if (node == target)
      {
        Gizmos.color = Color.red;
      }
      else
      {
        Gizmos.color = Color.blue;
      }
      Gizmos.DrawSphere(new Vector3(node.x, node.y, 0) + transform.position, 0.1f);
      Gizmos.color = Color.blue;

      if (y < height - 1 && node.neighbors.Contains(graph[x, y + 1]))
      {
        Gizmos.DrawLine(new Vector3(node.x, node.y, 0) + transform.position, new Vector3(node.x, node.y + 1, 0) + transform.position);
      }
      if (x < width - 1 && node.neighbors.Contains(graph[x + 1, y]))
      {
        Gizmos.DrawLine(new Vector3(node.x, node.y, 0) + transform.position, new Vector3(node.x + 1, node.y, 0) + transform.position);
      }


    }
  }

#if UNITY_EDITOR

  [CustomEditor(typeof(MazeGenerator))]
  public class MazeGeneratorEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      MazeGenerator generator = (MazeGenerator)target;
      base.OnInspectorGUI();
      if (GUILayout.Button("Re-Generate Maze"))
      {
        generator.StartCoroutine(generator.GenerateMaze());
      }
    }
  }
#endif
}