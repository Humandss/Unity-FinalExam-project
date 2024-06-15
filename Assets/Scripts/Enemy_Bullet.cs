using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    private MovementsTransform movement;
    private float projectileDistance = 40; // 적이 발사하는 총알의 최대 발사거리
    private int damage = 5;

   
    public void Setup(Vector3 position)
    {
        movement=GetComponent<MovementsTransform>();

        StartCoroutine("OnMove", position);
    }
    private IEnumerator OnMove(Vector3 targetPosition)
    {
        Vector3 start = transform.position;

        movement.MoveTo((targetPosition - transform.position).normalized);

        while(true)
        {
            if(Vector3.Distance(transform.position, start) >= projectileDistance)
            {
                Destroy(gameObject);

                yield break;
            }
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Player_Controller>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    
}
