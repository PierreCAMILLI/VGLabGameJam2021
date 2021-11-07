using System.Collections.Generic;
using UnityEngine;

public interface IPopPool
{
    public void OnPop();
}

public interface IPushPool
{
    public void OnPush();
}

public interface IDestroyPool
{
    public void OnDestroyInPool();
}

[System.Serializable]
public class Pool<T> where T : class
{
    private Stack<T> _pool;

    public int Size { get { return _pool.Count; } }

    protected virtual void OnPop(ref T item) { }
    protected virtual void OnPush(ref T item) { }
    protected virtual void OnDestroy(ref T item) { }

    public Pool()
    {
        _pool = new Stack<T>();
    }

    protected virtual T Create()
    {
        return default(T);
    }

    public bool Allocate(int size)
    {
        if (size < 0)
        {
            return false;
        }
        else if (size > _pool.Count)
        {
            do
            {
                Push(Create());
            } while (_pool.Count == size) ;
            _pool.TrimExcess();
            return true;
        }
        else if (size < _pool.Count)
        {
            do
            {
                PopAndDestroy();
            } while (_pool.Count == size);
            _pool.TrimExcess();
            return true;
        }
        return false;
    }

    private void PopAndDestroy()
    {
        T item = _pool.Pop();
        if (item is IDestroyPool)
        {
            (item as IDestroyPool).OnDestroyInPool();
        }
        OnDestroy(ref item);
    }

    public T Pop()
    {
        T item = _pool.Count > 0 ? _pool.Pop() : Create();
        if (item is IPopPool)
        {
            (item as IPopPool).OnPop();
        }
        OnPop(ref item);
        return item;
    }

    public T[] Pop(int count)
    {
        T[] items = new T[count];
        for (int i = 0; i < count; ++i)
        {
            items[i] = Pop();
        }
        return items;
    }

    public void Clear()
    {
        while (_pool.Count > 0)
        {
            PopAndDestroy();
        }
        _pool.TrimExcess();
    }

    public void Push(T item)
    {
        if (item != null)
        {
            OnPush(ref item);
            _pool.Push(item);
            if (item is IPushPool)
            {
                (item as IPushPool).OnPush();
            }
        }
    }

    public void Push(IEnumerable<T> items)
    {
        if (items != null)
        {
            foreach (T item in items)
            {
                Push(item);
            }
        }
    }
}

[System.Serializable]
public class PoolObject<T> : Pool<T> where T : UnityEngine.Object
{
    [SerializeField]
    private T _reference;
    public T Reference { get { return _reference; } }

    private System.Action<T> _onPop;
    private System.Action<T> _onPush;
    private System.Action<T> _onDestroy;

    private PoolObject() { }

    public PoolObject(T reference)
    {
        _reference = reference;
    }

    public void SetOnPop(System.Action<T> onPop)
    {
        _onPop = onPop;
    }

    public void SetOnPush(System.Action<T> onPush)
    {
        _onPush = onPush;
    }

    public void SetOnDestroy(System.Action<T> onDestroy)
    {
        _onDestroy = onDestroy;
    }

    protected override T Create()
    {
        return Object.Instantiate(_reference);
    }

    protected override void OnPop(ref T item)
    {
        if (item is Component)
        {
            (item as Component).gameObject.SetActive(true);
        }
        else if (item is GameObject)
        {
            (item as GameObject).SetActive(true);
        }
        if (_onPop != null)
        {
            _onPop(item);
        }
    }

    protected override void OnPush(ref T item)
    {
        if (_onPush != null)
        {
            _onPush(item);
        }
        if (item is Component)
        {
            (item as Component).gameObject.SetActive(false);
        }
        else if (item is GameObject)
        {
            (item as GameObject).SetActive(false);
        }
    }

    protected override void OnDestroy(ref T item)
    {
        if (_onDestroy != null)
        {
            _onDestroy(item);
        }
        if (item is Component)
        {
            Object.Destroy((item as Component).gameObject);
        }
        else
        {
            Object.Destroy(item);
        }
    }
}
