using UnityEngine;

public enum ImpactType { Normal = 0, Obstacle, Enemy, InteractionObject, }

public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab;
    private ObjectPool<Impact>[] pools;

    private void Awake()
    {
        pools = new ObjectPool<Impact>[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; ++i)
        {
            pools[i] = new ObjectPool<Impact>(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        if (hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("Enemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("InteractionObject"))
        {
            Color color = hit.transform.GetComponentInChildren<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
        }
    }

    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation, Color color = new Color())
    {
        Impact impact = pools[(int)type].Get();
        impact.transform.position = position;
        impact.transform.rotation = rotation;
        impact.Setup(pools[(int)type]);

        if (type == ImpactType.InteractionObject)
        {
            ParticleSystem.MainModule main = impact.GetComponent<ParticleSystem>().main;
            main.startColor = color;
        }
    }
}
