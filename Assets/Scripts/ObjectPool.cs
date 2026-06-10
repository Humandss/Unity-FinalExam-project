using System.Collections.Generic;
using UnityEngine;

// Generic, type-safe layer over MemoryPool.
//
// MemoryPool hands back a GameObject, which forces every caller to repeat
// GetComponent<T>() on each spawn. ObjectPool<T> returns the component
// directly and caches it per pooled instance, so the GetComponent cost is
// paid once per object instead of once per spawn. It also removes the
// stringly-typed, error-prone "which component does this pool hold?" knowledge
// from each call site.
public class ObjectPool<T> where T : Component
{
    private readonly MemoryPool pool;
    private readonly Dictionary<GameObject, T> components = new Dictionary<GameObject, T>();

    public int MaxCount => pool.MaxCount;
    public int ActiveCount => pool.ActiveCount;

    public ObjectPool(GameObject prefab)
    {
        pool = new MemoryPool(prefab);
    }

    public T Get()
    {
        GameObject go = pool.ActivatePoolItem();

        if (!components.TryGetValue(go, out T component))
        {
            component = go.GetComponent<T>();
            components[go] = component;
        }

        return component;
    }

    public void Release(T component)
    {
        if (component == null) return;
        pool.DeactivatePoolItem(component.gameObject);
    }

    public void ReleaseAll()
    {
        pool.DeactivateAllPoolItem();
    }
}
