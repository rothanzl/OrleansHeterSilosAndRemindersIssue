namespace Tester.Bomber;

public class ConstantMaxSizeList<T> : List<T>
{
    public int MaxSize { get; }

    public ConstantMaxSizeList(int maxSize)
    {
        if (maxSize < 0)
            throw new ArgumentOutOfRangeException(nameof(maxSize), "MaxSize must be greater or equal to 0");
        
        MaxSize = maxSize;
    }
    
    public new void Add(T item)
    {
        base.Add(item);
        ShrinkSize();
    }

    public new void AddRange(IEnumerable<T> collection)
    {
        base.AddRange(collection);
        ShrinkSize();
    }

    private void ShrinkSize()
    {
        while (Count > MaxSize)
        {
            RemoveAt(0);
        }
    }


}