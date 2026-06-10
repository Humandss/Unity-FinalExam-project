using System.Collections;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float deactivateTime = 5.0f;
    [SerializeField]
    private float casingSpin = 1.0f;
    [SerializeField]
    private AudioClip[] audioClips;

    private Rigidbody rigidbody3D;
    private ObjectPool<Casing> pool;
    private AudioSource audioSource;

    public void Setup(ObjectPool<Casing> pool, Vector3 direction)
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody3D = GetComponent<Rigidbody>();
        this.pool = pool;

        // launch and spin the casing
        rigidbody3D.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin));

        StartCoroutine(DeactivateAfterTime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);

        pool.Release(this);
    }
}
