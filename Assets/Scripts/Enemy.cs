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
    void EnemyDie() // �� ����� ȣ��Ǵ� �Լ�
    {
        if(enemyHealth <= 0)
        {
            print("�� ���!");
            Destroy(this.gameObject);
            Score_Manager.instance.IncreaseScore();
        }
    }
    void EnemyShooting() //���� ���� ��� �Լ�
    {
        if (enemyHealth > 0 && Time.time > enemyNextFireTime) 
        {
            Vector3 pos = this.gameObject.transform.position;

            enemyInstance = Instantiate(enemyBullet, new Vector3(pos.x, pos.y, (pos.z - 1.0f)), Quaternion.identity);

            enemyNextFireTime = Time.time + enemyFireRate;
        }
    }
    

}
