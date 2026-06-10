using UnityEngine;

// Stands still, then switches to Wander after a random delay.
// While idle it still reacts to the target through the context helper.
public class EnemyIdleState : IEnemyState
{
    private readonly EnemyFSM enemy;
    private float idleTimer;
    private float timeToWander;

    public EnemyState StateType => EnemyState.Idle;

    public EnemyIdleState(EnemyFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        idleTimer = 0f;
        timeToWander = Random.Range(1f, 5f);
    }

    public void Execute()
    {
        // Proximity to the target may move us to Pursuit / Attack / Wander.
        enemy.CalculateDistanceToTargetAndSelectState();
        if (enemy.CurrentState != EnemyState.Idle) return;

        idleTimer += Time.deltaTime;
        if (idleTimer >= timeToWander)
        {
            enemy.ChangeState(EnemyState.Wander);
        }
    }

    public void Exit() { }
}
