using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField]
    private float fadeSoeed = 4;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect");
    }
    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect");
    }

    private IEnumerator OnFadeEffect()
    {
        while (true)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSoeed, 1));
            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
/* public GameObject rangeObject;
    BoxCollider rangeCollider;
    public GameObject enemyPrefab;

    private void Awake()
    {
        rangeCollider = rangeObject.GetComponent<BoxCollider>();
    }
    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;

        float range_X = rangeCollider.bounds.size.x;
        float range_Z = rangeCollider.bounds.size.z;

        range_X = Random.Range((range_X/2)*-1, (range_X/2));
        range_Z = Random.Range((range_Z/2)*-1, (range_Z/2));
        Vector3 RandomPosition = new Vector3(range_X, 2f, range_Z);

        Vector3 respawnPosition = originPosition + RandomPosition;

        return respawnPosition;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RandomRespawn_Continue());
    }

    IEnumerator RandomRespawn_Continue()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            GameObject instantEnemy = Instantiate(enemyPrefab, Return_RandomPosition(), Quaternion.identity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }*/