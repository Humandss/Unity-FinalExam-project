using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject enemySpawnPointPrefab;  //적이 등장하기 전 적의 등장 위치를 알려주는 프리팹
    [SerializeField]
    private GameObject enemyPrefab; //적 프리팹
    [SerializeField]
    private float enemySpawnTime = 1; //적 생성 주기
    [SerializeField]
    private float enemySpawnLatency = 1; //타일 생성 후 적이 등장하기 까지 대기 시간

    private MemoryPool spawnPointMemoryPool; //적 등장 위치 알려주는 오브젝트 생성, 활성/비활성화 관리
    private MemoryPool enemyMemoryPool;// 적 생성, 활성/비활성 관리

    private int numberOfEnemiesSpawnedAtOnce = 1; // 동시 생성 가능한 적 숫자
    private Vector2Int mapSize = new Vector2Int(1000, 3000);

    // Start is called before the first frame update
    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }
    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while (true)
        {
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; i++)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();

                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 2f,
                                                      Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f));

                StartCoroutine("SpawnEnemy", item);
            }

            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }
    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        item.GetComponent<EnemyFSM>().Setup(target, this);

        spawnPointMemoryPool.DeactivatePoolItem(point);
    }
    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }
     
    // Update is called once per frame
    void Update()
    {
        
    }
}
