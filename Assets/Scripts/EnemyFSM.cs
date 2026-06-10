using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack }

// Context for the enemy State pattern.
//
// Holds the shared data/components and the currently active state object,
// and exposes intention-revealing helpers that the states call.
//
// The previous version drove the FSM with StartCoroutine(enemyState.ToString()):
//   * a typo in a state name failed silently (no coroutine, no error),
//   * the "Idle -> Wander after N seconds" coroutine was never stopped on a
//     state change, so it leaked and could yank the enemy back to Wander.
// Both problems disappear once each state owns its own behaviour and timers.
public class EnemyFSM : MonoBehaviour, IDamageable
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 30;   // recognition range
    [SerializeField]
    private float pursuitLimitiRange = 50;        // pursuit give-up range (name kept for Inspector compatibility)

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 20;
    [SerializeField]
    private float attackRate = 1;

    private MovementStatus status;
    private NavMeshAgent navMeshAgent;
    private Transform target;
    private EnemyMemoryPool enemyMemoryPool;
    private float lastAttackTime = 0;

    // ---- State pattern ----
    private readonly Dictionary<EnemyState, IEnemyState> states = new Dictionary<EnemyState, IEnemyState>();
    private IEnemyState currentState;
    private EnemyState currentStateType = EnemyState.None;

    public int enemyScore = 100;

    // ---- accessors used by the state objects ----
    public EnemyState CurrentState => currentStateType;
    internal NavMeshAgent Agent => navMeshAgent;
    internal MovementStatus Status => status;
    internal Transform Target => target;

    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<MovementStatus>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        navMeshAgent.updateRotation = false;

        BuildStates();
        ChangeState(EnemyState.Idle);
    }

    private void BuildStates()
    {
        if (states.Count > 0) return;

        states.Add(EnemyState.Idle, new EnemyIdleState(this));
        states.Add(EnemyState.Wander, new EnemyWanderState(this));
        states.Add(EnemyState.Pursuit, new EnemyPursuitState(this));
        states.Add(EnemyState.Attack, new EnemyAttackState(this));
    }

    private void OnEnable()
    {
        // Reused from the pool: restart at Idle if the states already exist.
        // (On the very first spawn, Setup() starts the FSM instead, because
        //  OnEnable runs before Setup() when the pool activates the object.)
        if (states.Count > 0)
        {
            ChangeState(EnemyState.Idle);
        }
    }

    private void OnDisable()
    {
        currentState?.Exit();
        currentState = null;
        currentStateType = EnemyState.None;
    }

    private void Update()
    {
        currentState?.Execute();
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentStateType == newState) return;

        currentState?.Exit();

        if (newState == EnemyState.None || states.Count == 0)
        {
            currentState = null;
            currentStateType = EnemyState.None;
            return;
        }

        currentStateType = newState;
        currentState = states[newState];
        currentState.Enter();
    }

    internal void LookRotationToTarget()
    {
        if (target == null) return;

        Vector3 to = new Vector3(target.position.x, 0f, target.position.z);
        Vector3 from = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 direction = to - from;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    internal void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if (distance >= pursuitLimitiRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    internal void TryFireProjectile()
    {
        if (target == null || projectilePrefab == null || projectileSpawnPoint == null) return;
        if (Time.time - lastAttackTime <= attackRate) return;

        lastAttackTime = Time.time;

        GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        clone.GetComponent<Enemy_Bullet>().Setup(target.position);
    }

    internal Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;        // radius around the current position
        int wanderJitterMin = 0;        // minimum angle
        int wanderJitterMax = 360;      // maximum angle

        Vector3 rangerPosition = Vector3.zero;          // centre of the playable area
        Vector3 rangeScale = Vector3.one * 100.0f;      // size of the playable area

        int wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        targetPosition.x = Mathf.Clamp(targetPosition.x, rangerPosition.x - rangeScale.x * 0.5f, rangerPosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangerPosition.z - rangeScale.z * 0.5f, rangerPosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, float angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if (isDie)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
            GameEvents.RaiseEnemyKilled(enemyScore);
        }
    }

    private void OnDrawGizmos()
    {
        // Guard navMeshAgent: it is only assigned at runtime in Setup(),
        // so in the Scene view (edit mode) it is null.
        if (navMeshAgent != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitiRange);

        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
