using UnityEngine;

// Legacy top-down shooter enemy (Module A).
//
// Refactored to apply the same two patterns used in the FPS module:
//   * IDamageable  - bullets damage the enemy they actually hit, instead of
//                    reaching through a bogus Enemy.instance singleton (which
//                    pointed at one fixed enemy, not the one that was hit).
//   * Observer     - on death it raises GameEvents.EnemyKilled(enemyScore)
//                    instead of calling Score_Manager directly.
public class Enemy : MonoBehaviour, IDamageable
{
    public int enemyHealth = 100;
    public int enemyScore = 100;
    public GameObject enemyBullet;
    public Transform enemyFirePos;

    private float enemyFireRate = 0.4f;
    private float enemyNextFireTime = 0f;

    private void Update()
    {
        EnemyShooting();
    }

    public void TakeDamage(int damage)
    {
        if (enemyHealth <= 0) return;

        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            GameEvents.RaiseEnemyKilled(enemyScore);
            Destroy(gameObject);
        }
    }

    private void EnemyShooting()
    {
        if (enemyHealth > 0 && Time.time > enemyNextFireTime)
        {
            Vector3 pos = transform.position;
            Instantiate(enemyBullet, new Vector3(pos.x, pos.y, pos.z - 1.0f), Quaternion.identity);

            enemyNextFireTime = Time.time + enemyFireRate;
        }
    }
}
