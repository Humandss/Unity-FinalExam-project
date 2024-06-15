using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float explosionDelayTime = 0.3f;
    [SerializeField]
    private float explosionRadius = 30.0f;
    [SerializeField]
    private float explosionForce = 1000000.0f;

    private bool isExplode = false;

    public override void TakeDamage(int damage)
    {
        currentHP-=damage;

        if (currentHP <= 0 && isExplode == false) 
        {
            StartCoroutine("ExplodeBarrel");
        }
    }    
   private IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(explosionDelayTime);
        //근처의 배럴이 터져서의 다시 현재 배럴을 터뜨릴때
        isExplode = true;
        //폭발 이펙트 생성
        Bounds bounds =GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);
        //폭발 범위에 잇는 모든 오브젝트 collider 정보 받아와 폭발 효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in colliders)
        {
            //폭발 범위 안에 플레이어가 들어갈 시
            Player_Controller player = hit.GetComponent<Player_Controller>();
            if(player != null)
            {
                player.TakeDamage(50);
                continue;
                    
            }
            // 폭발 범위 안에 적이 들어갈 시
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if(enemy!=null)
            {
                enemy.TakeDamage(300); continue;
            }
            //폭발 범위안에 오브젝트 존재할시
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction!=null)
            {
                interaction.TakeDamage(100); continue;
            }
            //중력을 가지고 있는 오브젝트이면 밀려나게 설정
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position,explosionRadius);    
            }
        }

        Destroy(gameObject);
    }
}
