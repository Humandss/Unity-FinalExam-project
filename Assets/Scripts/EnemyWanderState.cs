using UnityEngine;

// Walks to a random reachable position; returns to Idle on arrival or timeout.
public class EnemyWanderState : IEnemyState
{
    private const float MaxWanderTime = 10f;

    private readonly EnemyFSM enemy;
    private float elapsed;

    public EnemyState StateType => EnemyState.Wander;

    public EnemyWanderState(EnemyFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        elapsed = 0f;

        enemy.Agent.speed = enemy.Status.WalkSpeed;
        enemy.Agent.SetDestination(enemy.CalculateWanderPosition());

        Vector3 to = new Vector3(enemy.Agent.destination.x, 0f, enemy.Agent.destination.z);
        Vector3 from = new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z);
        if ((to - from).sqrMagnitude > 0.0001f)
        {
            enemy.transform.rotation = Quaternion.LookRotation(to - from);
        }
    }

    public void Execute()
    {
        elapsed += Time.deltaTime;

        Vector3 to = new Vector3(enemy.Agent.destination.x, 0f, enemy.Agent.destination.z);
        Vector3 from = new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z);

        if ((to - from).sqrMagnitude < 0.01f || elapsed >= MaxWanderTime)
        {
            enemy.ChangeState(EnemyState.Idle);
            return;
        }

        enemy.CalculateDistanceToTargetAndSelectState();
    }

    public void Exit() { }
}
