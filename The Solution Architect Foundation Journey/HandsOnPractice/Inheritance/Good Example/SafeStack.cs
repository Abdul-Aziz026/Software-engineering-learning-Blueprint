
namespace Inheritance;

internal class SafeStack<T>
{
    private readonly List<T> _items = new();
    public int Count => _items.Count;

    public void Push(T item) => _items.Add(item);
    public void Pop()
    {
        if (_items.Count == 0) throw new InvalidOperationException("Stack is empty.");
        _items.RemoveAt(_items.Count - 1);
    }

    public T Peek()
    {
        if (_items.Count == 0) throw new InvalidOperationException("Stack is empty.");
        return _items[_items.Count - 1];
    }
}
