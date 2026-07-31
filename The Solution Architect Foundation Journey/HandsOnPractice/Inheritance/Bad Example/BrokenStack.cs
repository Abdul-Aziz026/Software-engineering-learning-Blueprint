using System;
using System.Collections.Generic;
using System.Text;

namespace Inheritance;

internal class BrokenStack<T> : List<T>
{
    public void Push(T item) => Add(item);
    public void Pop()
    {
        if (Count == 0) throw new InvalidOperationException("Stack is empty.");
        RemoveAt(Count - 1);
    }
    public T Peek()
    {
        if (Count == 0) throw new InvalidOperationException("Stack is empty.");
        return this[Count - 1];
    }
}
