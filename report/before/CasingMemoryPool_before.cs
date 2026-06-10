using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject casingPrefabe;
    private MemoryPool memoryPool;

    // Start is called before the first frame update
    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefabe);
    }
    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(memoryPool,direction);
    }
}
