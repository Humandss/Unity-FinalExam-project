using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Bullet : MonoBehaviour
{


    public float bullet_Speed = 4.0f;
    public int bullet_Damage = 20;
  
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * bullet_Speed * Time.deltaTime * 2.0f);

        if (this.gameObject.transform.position.z >= 70.0f)
        {
            Destroy(this.gameObject);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (Enemy.instance.enemyHealth != 0)
            {
                Enemy.instance.enemyHealth = Enemy.instance.enemyHealth - bullet_Damage;
            }
            print(Enemy.instance.enemyHealth);
            Destroy(this.gameObject);
            
        }
        if (other.CompareTag("Bullet"))
        {
            Destroy(this.gameObject);
        }

    }
  
}
