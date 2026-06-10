using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private ObjectPool<Impact> pool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Setup(ObjectPool<Impact> pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        if (particle.isPlaying == false)
        {
            pool.Release(this);
        }
    }
}
