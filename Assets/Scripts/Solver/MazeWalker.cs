using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum State { Waiting, Searching, Moving, End }
public class MazeWalker : MonoBehaviour
{
  public Vector3 offset;
  private MazeSolver solver;
  public MazeGenerator maze;
  private new SpriteRenderer renderer;
  private State state;
  public Transform searchIndicator;
  public float searchTime = 0.5f;
  public float walkTime = 0.5f;
  private Vector3 target;
  private float walkProgress;
  public GameObject checkedMarkerPrefab;
  private List<GameObject> checkedMarkers = new();
  private void Start()
  {
    renderer = GetComponent<SpriteRenderer>();
    searchIndicator.gameObject.SetActive(false);
    renderer.enabled = false;
    solver = GetComponent<MazeSolver>();
    state = State.Waiting;
    maze.onMazeGenerated.AddListener(OnMazeGenerated);

    solver.onPathFound.AddListener(OnPathFound);
  }

  private void OnMazeGenerated()
  {
    checkedMarkers.ForEach(Destroy);
    checkedMarkers.Clear();
    state = State.Searching;
    renderer.enabled = true;
    target = new Vector3(maze.start.x, maze.start.y, 0) + maze.transform.position + offset;
    transform.position = target;
    searchIndicator.gameObject.SetActive(true);
    StartCoroutine(SearchPath());
  }
  private void OnPathFound(List<Node> path)
  {
    state = State.Moving;
    StartCoroutine(WalkToEnd(path));
  }

  private void Update()
  {
    if (state == State.Moving)
    {
      walkProgress += Time.deltaTime / walkTime;
      if (walkProgress >= 1)
      {
        walkProgress = 1;
      }
      transform.position = Vector3.Lerp(transform.position, target, walkProgress);
    }
  }

  private IEnumerator SearchPath()
  {
    yield return new WaitForSeconds(searchTime);
    var it = solver.AStar();
    while (it.MoveNext())
    {
      var node = it.Current;
      searchIndicator.position = new Vector3(node.x, node.y, 0) + maze.transform.position;
      checkedMarkers.Add(Instantiate(checkedMarkerPrefab, searchIndicator.position, Quaternion.identity));
      yield return new WaitForSeconds(searchTime);
    }



  }

  private IEnumerator WalkToEnd(List<Node> path)
  {
    foreach (var node in path)
    {
      target = new Vector3(node.x, node.y, 0) + maze.transform.position + offset;
      walkProgress = 0;
      yield return new WaitForSeconds(walkTime);
    }
  }
}