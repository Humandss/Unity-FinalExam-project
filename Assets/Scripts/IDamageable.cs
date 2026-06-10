// Strategy-style abstraction for "anything that can take damage".
//
// Lets weapons, explosions and bullets deal damage polymorphically:
//     hit.GetComponent<IDamageable>()?.TakeDamage(amount);
// instead of branching on CompareTag(...) + GetComponent<ConcreteType>()
// for every damageable type (Enemy, EnemyFSM, InteractionObject, Player...).
// Adding a new damageable type no longer means editing every weapon.
public interface IDamageable
{
    void TakeDamage(int damage);
}
