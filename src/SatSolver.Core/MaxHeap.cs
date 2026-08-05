namespace SatSolver.Core;

/// <summary>
/// A specialized max binary heap that can contain the integers 1..N (without
/// duplicates) and tracks the position of each integer in the backing array.
/// This structure allows the client to update a position of an element in the
/// heap by calling <see cref="UpHeap(int)"/> or <see cref="DownHeap(int)"/>
/// after the relative position of an element has changed.
/// </summary>
public class MaxHeap
{
    /// <summary>
    /// The backing array, values are indexed starting from 1. Value if _heap[0]
    /// will always be equal to zero. The root is _heap[1], the child nodes of
    /// _heap[n] are _heap[2*n] and _heap[2*n + 1].
    /// </summary>
    private readonly int[] _heap;

    /// <summary>
    /// An array that stores the index of each value in _heap. For example,
    /// _pos[3] will contain the index of value 3 in _heap. If a value is not
    /// present in the heap, the returned value will be -1.
    /// </summary>
    private readonly int[] _pos;
    private int _tail;
    private readonly Comparison<int> _comp;

    /// <summary>
    /// Initializes a new instance of MaxHeap class with N values and a given
    /// comparison method. The heap is initialized to contain the values 1, 2,
    /// ... n.
    /// </summary>
    /// <param name="n">The number of values in the heap.</param>
    /// <param name="comparison">The comparison method to use.</param>
    private MaxHeap(int n, Comparison<int> comparison)
    {
        _heap = new int[n + 1];
        _pos = new int[n + 1];
        _comp = comparison;
    }

    /// <summary>
    /// Creates an instance of <see cref="MaxHeap"/> which contains zero elements.
    /// </summary>
    /// <param name="n">The maximum number of elements that the created instance
    /// can support.</param>
    /// <returns>The created <see cref="MaxHeap"/> instance.</returns>
    public static MaxHeap Empty(int n)
    {
        return Empty(n, (a, b) => a - b);
    }

    /// <summary>
    /// Creates an instance of <see cref="MaxHeap"/> which contains zero elements.
    /// </summary>
    /// <param name="n">The maximum number of elements that the created instance
    /// can support.</param>
    /// <param name="comparison">The comparison method to use.</param>
    /// <returns>The created <see cref="MaxHeap"/> instance.</returns>
    public static MaxHeap Empty(int n, Comparison<int> comparison)
    {
        MaxHeap heap = new(n, comparison)
        {
            _tail = 1
        };

        for (int i = 1; i <= n; ++i)
        {
            heap._pos[i] = -1;
        }

        return heap;
    }

    /// <summary>
    /// Creates a new instance of MaxHeap class which contains the elements 1, 2, ..., n.
    /// </summary>
    /// <param name="n">The maximum number of elements that the created instance
    /// can support.</param>
    /// <returns>The created <see cref="MaxHeap"/> instance.</returns>
    public static MaxHeap Create(int n)
    {
        return Create(n, (a, b) => a - b);
    }

    /// <summary>
    /// Creates a new instance of MaxHeap class which contains the elements 1, 2, ..., n.
    /// </summary>
    /// <param name="n">The maximum number of elements that the created instance
    /// can support.</param>
    /// <param name="comparison">The comparison method to use.</param>
    /// <returns>The created <see cref="MaxHeap"/> instance.</returns>
    public static MaxHeap Create(int n, Comparison<int> comparison)
    {
        MaxHeap heap = new(n, comparison)
        {
            _tail = n + 1
        };

        for (int i = 1; i <= n; ++i)
        {
            heap._heap[i] = i;
            heap._pos[i] = i;
        }

        int index = n / 2;

        while (index > 0)
        {
            heap.HeapifyDown(index);
            --index;
        }

        return heap;
    }

    /// <summary>
    /// Adds the specified element to the <see cref="MaxHeap"/>.
    /// </summary>
    /// <param name="value">The element to be added.</param>
    /// <returns><c>true</c> if the element is added to the <see
    /// cref="MaxHeap"/>; <c>false</c> if the element is already
    /// present.</returns>
    public bool Push(int value)
    {
        if (_pos[value] != -1)
        {
            return false;
        }

        _heap[_tail] = value;
        _pos[value] = _tail;
        HeapifyUp(_tail);
        _tail++;

        return true;
    }

    /// <summary>
    /// Removes and returns the maximum element of the <see cref="MaxHeap"/>.
    /// </summary>
    /// <returns>The maximum element of <see cref="MaxHeap"/>.</returns>
    public int Pop()
    {
        int ret = _heap[1];
        _tail--;
        _heap[1] = _heap[_tail];
        _heap[_tail] = 0;
        _pos[_heap[1]] = 1;
        _pos[ret] = -1;
        HeapifyDown(1);
        return ret;
    }

    /// <summary>
    /// Perform the 'up-heap' operation on a given element when it is known that
    /// the relative position of the value has increased.
    /// </summary>
    /// <param name="element">The value for which the relative position needs to be updated.</param>
    public void UpHeap(int element)
    {
        HeapifyUp(_pos[element]);
    }

    /// <summary>
    /// Perform the 'down-heap' operation on a given element when it is known
    /// that the relative position of the value has decreased.
    /// </summary>
    /// <param name="element">The value for which the relative position needs to be updated.</param>
    public void DownHeap(int element)
    {
        HeapifyDown(_pos[element]);
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            int left = index * 2;
            int right = index * 2 + 1;
            int largest = index;

            if (left < _tail && _comp(_heap[left], _heap[largest]) > 0)
            {
                largest = left;
            }

            if (right < _tail && _comp(_heap[right], _heap[largest]) > 0)
            {
                largest = right;
            }

            if (largest == index)
            {
                break;
            }

            Swap(index, largest);
            index = largest;
        }
    }

    private void HeapifyUp(int index)
    {
        int parent = index / 2;

        while (index > 1 && _comp(_heap[index], _heap[parent]) > 0)
        {
            Swap(index, parent);
            index = parent;
            parent = index / 2;
        }
    }

    private void Swap(int i, int j)
    {
        (_pos[_heap[j]], _pos[_heap[i]]) = (_pos[_heap[i]], _pos[_heap[j]]);
        (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
    }
}
