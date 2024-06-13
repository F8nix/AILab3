using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<T> : IEnumerable<T>
{
  private List<(int priority, T item)> heap;

  public PriorityQueue()
  {
    heap = new List<(int, T)>();
  }

  public int Count => heap.Count;

  public void Enqueue(int priority, T item)
  {
    heap.Add((priority, item));
    HeapifyUp(heap.Count - 1);
  }

  public T Dequeue()
  {
    if (heap.Count == 0) throw new InvalidOperationException("The queue is empty.");

    var root = heap[0].item;
    heap[0] = heap[^1];
    heap.RemoveAt(heap.Count - 1);

    HeapifyDown(0);

    return root;
  }

  public T Peek()
  {
    if (heap.Count == 0) throw new InvalidOperationException("The queue is empty.");
    return heap[0].item;
  }

  public bool IsEmpty()
  {
    return heap.Count == 0;
  }

  private void HeapifyUp(int index)
  {
    while (index > 0)
    {
      int parentIndex = (index - 1) / 2;

      if (heap[index].priority >= heap[parentIndex].priority)
        break;

      Swap(index, parentIndex);
      index = parentIndex;
    }
  }

  private void HeapifyDown(int index)
  {
    while (index < heap.Count)
    {
      int leftChild = 2 * index + 1;
      int rightChild = 2 * index + 2;
      int smallest = index;

      if (leftChild < heap.Count && heap[leftChild].priority < heap[smallest].priority)
        smallest = leftChild;

      if (rightChild < heap.Count && heap[rightChild].priority < heap[smallest].priority)
        smallest = rightChild;

      if (smallest == index)
        break;

      Swap(index, smallest);
      index = smallest;
    }
  }

  private void Swap(int index1, int index2)
  {
    (heap[index2], heap[index1]) = (heap[index1], heap[index2]);
  }

  public IEnumerator<T> GetEnumerator()
  {
    foreach (var (_, item) in heap)
      yield return item;
  }


  IEnumerator IEnumerable.GetEnumerator()
  {
    foreach (var (_, item) in heap)
      yield return item;
  }
}