namespace SatSolver.Core.Tests;

[Trait("Category", "Unit")]
public class MaxHeapTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void Create_ShouldInitializeHeapWithValues(int n)
    {
        MaxHeap heap = MaxHeap.Create(n);

        for (int v = n; v > 0; --v)
        {
            Assert.Equal(v, heap.Pop());
        }
    }

    [Fact]
    public void Empty_ShouldInitializeEmptyHeap()
    {
        MaxHeap heap = MaxHeap.Empty(5);

        heap.Push(3);
        heap.Push(2);
        heap.Push(4);
        heap.Push(1);
        heap.Push(5);

        Assert.Equal(5, heap.Pop());
        Assert.Equal(4, heap.Pop());
        Assert.Equal(3, heap.Pop());
        Assert.Equal(2, heap.Pop());
        Assert.Equal(1, heap.Pop());
    }

    [Fact]
    public void Create_ShouldUseComparisonMethod()
    {
        double[] scores = [0.0, 2.5, 7.2, 8.3, 9.7, 5.1];

        MaxHeap heap = MaxHeap.Create(5, (a, b) => scores[a] < scores[b] ? -1 : 1);

        Assert.Equal(4, heap.Pop());
        Assert.Equal(3, heap.Pop());
        Assert.Equal(2, heap.Pop());
        Assert.Equal(5, heap.Pop());
        Assert.Equal(1, heap.Pop());
    }

    [Fact]
    public void Empty_ShouldUseComparisonMethod()
    {
        double[] scores = [0.0, 2.5, 7.2, 8.3, 9.7, 5.1];

        MaxHeap heap = MaxHeap.Empty(5, (a, b) => scores[a] < scores[b] ? -1 : 1);

        heap.Push(3);
        heap.Push(2);
        heap.Push(4);
        heap.Push(1);
        heap.Push(5);

        Assert.Equal(4, heap.Pop());
        Assert.Equal(3, heap.Pop());
        Assert.Equal(2, heap.Pop());
        Assert.Equal(5, heap.Pop());
        Assert.Equal(1, heap.Pop());
    }

    [Fact]
    public void UpHeap_ShouldReorderHeap()
    {
        double[] scores = [0.0, 2.5, 7.2, 8.3, 9.7, 5.1];

        MaxHeap heap = MaxHeap.Create(5, (a, b) => scores[a] < scores[b] ? -1 : 1);

        scores[2] = 100.0;
        heap.UpHeap(2);

        Assert.Equal(2, heap.Pop());
        Assert.Equal(4, heap.Pop());
        Assert.Equal(3, heap.Pop());
        Assert.Equal(5, heap.Pop());
        Assert.Equal(1, heap.Pop());
    }

    [Fact]
    public void DownHeap_ShouldReorderHeap()
    {
        double[] scores = [0.0, 2.5, 7.2, 8.3, 9.7, 5.1];

        MaxHeap heap = MaxHeap.Create(5, (a, b) => scores[a] < scores[b] ? -1 : 1);

        scores[2] = 0.0;
        heap.DownHeap(2);

        Assert.Equal(4, heap.Pop());
        Assert.Equal(3, heap.Pop());
        Assert.Equal(5, heap.Pop());
        Assert.Equal(1, heap.Pop());
        Assert.Equal(2, heap.Pop());
    }

    [Fact]
    public void Push_ShouldReturnFalse_WhenHeapAlreadyContainsValue()
    {
        MaxHeap heap = MaxHeap.Empty(5);

        Assert.True(heap.Push(3));
        Assert.False(heap.Push(3));
    }
}
