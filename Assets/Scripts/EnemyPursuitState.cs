using UnityEngine;

// Chases the target at run speed until it is in attack range or out of sight.
public class EnemyPursuitState : IEnemyState
{
    private readonly EnemyFSM enemy;

    public EnemyState StateType => EnemyState.Pursuit;

    public EnemyPursuitState(EnemyFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Agent.speed = enemy.Status.RunSpeed;
    }

    public void Execute()
    {
        if (enemy.Target == null) return;

        enemy.Agent.SetDestination(enemy.Target.position);
        enemy.LookRotationToTarget();
        enemy.CalculateDistanceToTargetAndSelectState();
    }

    public void Exit() { }
}
