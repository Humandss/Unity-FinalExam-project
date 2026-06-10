using UnityEngine;

// Stops moving, faces the target and fires projectiles on a fixed cadence.
public class EnemyAttackState : IEnemyState
{
    private readonly EnemyFSM enemy;

    public EnemyState StateType => EnemyState.Attack;

    public EnemyAttackState(EnemyFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Agent.ResetPath();
    }

    public void Execute()
    {
        if (enemy.Target == null) return;

        enemy.LookRotationToTarget();
        enemy.CalculateDistanceToTargetAndSelectState();
        enemy.TryFireProjectile();
    }

    public void Exit() { }
}
