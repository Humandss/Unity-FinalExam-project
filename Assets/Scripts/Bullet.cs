using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bullet_Speed = 4.0f;
    public int bullet_Damage = 20;

    private void Update()
    {
        transform.Translate(Vector3.forward * bullet_Speed * Time.deltaTime * 2.0f);

        if (transform.position.z >= 70.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Polymorphic damage via IDamageable: hit whatever we actually
        // collided with, instead of Enemy.instance (the old singleton bug).
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(bullet_Damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
