using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public int enemyHealth = 100;
    public int enemyScore = 100;
    public GameObject enemyBullet;
    public Transform enemyFirePos;
    GameObject enemyInstance;

    private float enemyFireRate = 0.4f;
    private float enemyNextFireTime = 0f;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Enemy.instance == null)
        {
            Enemy.instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        EnemyShooting();
        EnemyDie();
    }
    void EnemyDie() // 적 사망시 호출되는 함수
    {
        if(enemyHealth <= 0)
        {
            print("적 사망!");
            Destroy(this.gameObject);
            Score_Manager.instance.IncreaseScore();
        }
    }
    void EnemyShooting() //적이 총을 쏘는 함수
    {
        if (enemyHealth > 0 && Time.time > enemyNextFireTime) 
        {
            Vector3 pos = this.gameObject.transform.position;

            enemyInstance = Instantiate(enemyBullet, new Vector3(pos.x, pos.y, (pos.z - 1.0f)), Quaternion.identity);

            enemyNextFireTime = Time.time + enemyFireRate;
        }
    }
    

}
