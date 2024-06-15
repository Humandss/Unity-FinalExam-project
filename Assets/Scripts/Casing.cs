using System.Collections;
using System.Collections.Generic;
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
    private MemoryPool memoryPool;
    private AudioSource audioSource;

    public void Setup(MemoryPool pool, Vector3 direction)
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody3D = GetComponent<Rigidbody>();
        memoryPool = pool;

        //탄피 이동 속력과 회전 속력 설정
        rigidbody3D.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin));

        StartCoroutine("DeactivateAfterTIme");
    }
    private void OnCollisionEnter(Collision collision)
    {
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
    private IEnumerator DeactivateAfterTIme()
    {
        yield return new WaitForSeconds(deactivateTime);

        memoryPool.DeactivatePoolItem(this.gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
