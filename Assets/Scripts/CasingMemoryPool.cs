using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject casingPrefabe; // name kept for Inspector compatibility
    private ObjectPool<Casing> pool;

    private void Awake()
    {
        pool = new ObjectPool<Casing>(casingPrefabe);
    }

    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        Casing casing = pool.Get();
        casing.transform.position = position;
        casing.transform.rotation = Random.rotation;
        casing.Setup(pool, direction);
    }
}
