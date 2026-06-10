using System.Collections.Generic;
using UnityEngine;

// Object Pool pattern.
//
// Reuses inactive GameObjects instead of Instantiate/Destroy (which churn the
// GC and cause frame hitches). The public API is unchanged, but the naive
// implementation had two problems that are fixed here:
//   * ActivatePoolItem / DeactivatePoolItem were O(n) linear scans of every
//     pooled item. They are now O(1): a queue hands out free items and a
//     GameObject -> item dictionary finds the one to release.
//   * DeactivateAllPoolItem iterated [0, activeCount) over the full list, so
//     it deactivated the wrong items whenever the active ones were not packed
//     at the front. It now walks the items that are actually active.
public class MemoryPool
{
    private class PoolItem
    {
        public bool isActive;        // is the object currently in use
        public GameObject gameObject; // the pooled instance
    }

    private readonly int increaseCount = 5; // how many to add when the pool runs dry
    private int maxCount;                    // total instances owned by the pool
    private int activeCount;                 // instances currently in use

    private readonly GameObject poolObject;
    private readonly List<PoolItem> poolItemList = new List<PoolItem>();
    private readonly Queue<PoolItem> inactiveItems = new Queue<PoolItem>();              // free list -> O(1) activate
    private readonly Dictionary<GameObject, PoolItem> lookup = new Dictionary<GameObject, PoolItem>(); // O(1) release

    public int MaxCount => maxCount;
    public int ActiveCount => activeCount;

    private readonly Vector3 tempPosition = new Vector3(48, 1, 48); // parking spot for inactive items

    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        InstantiateObjects();
    }

    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; i++)
        {
            PoolItem poolItem = new PoolItem
            {
                isActive = false,
                gameObject = GameObject.Instantiate(poolObject)
            };
            poolItem.gameObject.transform.position = tempPosition;
            poolItem.gameObject.SetActive(false);

            poolItemList.Add(poolItem);
            inactiveItems.Enqueue(poolItem);
            lookup.Add(poolItem.gameObject, poolItem);
        }
    }

    public void DestroyObjects()
    {
        if (poolItemList.Count == 0) return;

        foreach (PoolItem poolItem in poolItemList)
        {
            GameObject.Destroy(poolItem.gameObject);
        }

        poolItemList.Clear();
        inactiveItems.Clear();
        lookup.Clear();
        maxCount = 0;
        activeCount = 0;
    }

    public GameObject ActivatePoolItem()
    {
        if (inactiveItems.Count == 0)
        {
            InstantiateObjects(); // grow the pool; new items are enqueued as free
        }

        PoolItem poolItem = inactiveItems.Dequeue();
        activeCount++;

        poolItem.isActive = true;
        poolItem.gameObject.SetActive(true);

        return poolItem.gameObject;
    }

    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (removeObject == null) return;
        if (!lookup.TryGetValue(removeObject, out PoolItem poolItem)) return;
        if (!poolItem.isActive) return; // already released; avoid double counting

        activeCount--;
        poolItem.isActive = false;
        poolItem.gameObject.transform.position = tempPosition;
        poolItem.gameObject.SetActive(false);

        inactiveItems.Enqueue(poolItem);
    }

    public void DeactivateAllPoolItem()
    {
        foreach (PoolItem poolItem in poolItemList)
        {
            if (poolItem.gameObject != null && poolItem.isActive)
            {
                poolItem.isActive = false;
                poolItem.gameObject.transform.position = tempPosition;
                poolItem.gameObject.SetActive(false);

                inactiveItems.Enqueue(poolItem);
            }
        }
        activeCount = 0;
    }
}
