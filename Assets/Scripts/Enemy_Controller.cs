using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    public float moveSpeed = 6.0f;
    public Transform target;
    public float detectionRange = 100.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TargetingPlayer();

        SelfDestroyEnemy();
    }
    void TargetingPlayer() // 플레이어 타게팅 함수
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= detectionRange)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
    }
    void SelfDestroyEnemy()
    {
        if (this.gameObject.transform.position.z <= -48.0f)
        {
            Destroy(this.gameObject);
        }
    }
    
}
