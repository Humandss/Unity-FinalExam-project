using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack }
public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 30; //�ν� ����
    [SerializeField]
    private float pursuitLimitiRange = 50; // ���� ����

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 20;
    [SerializeField]
    private float attackRate = 1;

    private EnemyState enemyState = EnemyState.None; //���� �� �ൿ 
    private float lastAttackTime = 0;   // ���� �ֱ� ��� ����

    private MovementStatus status; // �̵��ӵ� ���� ����
    private NavMeshAgent navMeshAgent; // �̵� ��� ���� �׺���̼� �޽�
    private Transform target; //���ݴ�� => �÷��̾�
    private EnemyMemoryPool enemyMemoryPool; // �� �޸� Ǯ
    //private Score_Manager manager;

    public int enemyScore = 100;
    public static EnemyFSM instance;
    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status=GetComponent<MovementStatus>();
        navMeshAgent=GetComponent<NavMeshAgent>();
        //manager=GetComponent<Score_Manager>();
        
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        navMeshAgent.updateRotation = false;
    }
    private void Awake()
    {
        if (EnemyFSM.instance == null)
        {
            EnemyFSM.instance = this;
        }
    }
    private void OnEnable()
    { //ó�� �����¸� �����·� ����
        ChangeState(EnemyState.Idle);
    }
    private void OnDisable()
    {
        //���� ��Ȱ��ȭ��� ���� ������� ���·� ����, ���¸� none���� ����
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }
    public void ChangeState(EnemyState newState)
    {
        //���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ����
        if(enemyState == newState) return;
        //���� ������� ���� ����
        StopCoroutine(enemyState.ToString());

        enemyState=newState;
        //���ο� �������
        StartCoroutine(enemyState.ToString());
    }
    private IEnumerator Idle()
    {
        //n�� �Ŀ� "��ȸ"���·� �����ϴ� �ڷ�ƾ
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            //Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����
            CalculateDistanceToTargetAndSelectState();
            //��� �����϶� �ϴ� �ൿ
            yield return null;
        }
    }
    private IEnumerator AutoChangeFromIdleToWander()
    {
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        ChangeState(EnemyState.Wander);
    }
    
    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        //�̵� �ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;
        //��ǥ ��ġ ���� 
        navMeshAgent.SetDestination(CalculateWanderPosition());

        //��ǥ ��ġ�� ȸ�� 
        Vector3 to = new Vector3(navMeshAgent.destination.x,0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation=Quaternion.LookRotation(to-from);

        while(true)
        {
            currentTime += Time.deltaTime;

            //��ǥ ��ġ�� �����Ͽ� �����ϰų� �ʹ� �����ð����� ��ȸ�ϱ� ���¿� �ӹ��� ������
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x,0,transform.position.z);

            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime) 
            {
                ChangeState(EnemyState.Idle);
            }
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }
    
    private IEnumerator Attack()
    {
        
        navMeshAgent.ResetPath();

        while(true)
        {
            LookRotationToTarget();

            CalculateDistanceToTargetAndSelectState();

            if(Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<Enemy_Bullet>().Setup(target.position);
            }
            yield return null;
        }
    }
    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10; // ���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0; // ���õ� ����
        int wanderJitterMin = 0; // �ּ� ����
        int wanderJitterMax = 360; //�ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ��
        Vector3 rangerPosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        //�ڽ��� ��ġ�� �߽����� ������ �Ÿ�, ���õ� ������ ��ġ�� ��ǥ�� ��ǥ �������� ����
        wanderJitter= Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        //������ ��ǥ ��ġ�� �ڽ��� �̵� ������ ����� �ʰ� ����
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
    private IEnumerator Pursuit()
    {
        while(true)
        {
            //�̵��ӵ� ����
            navMeshAgent.speed = status.RunSpeed;
            // ��ǥ ��ġ�� ���� �÷��̾� ��ġ�� ����
            navMeshAgent.SetDestination(target.position);
            // Ÿ�� ������ �ֽ�
            LookRotationToTarget();
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
        
        
    }
    private void LookRotationToTarget()
    {
        //��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x,0, target.position.z);
        // �÷��̾� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);


        transform.rotation = Quaternion.LookRotation(to-from); 
    }
    private void CalculateDistanceToTargetAndSelectState()
    {
        if(target==null) return;

        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= attackRange)
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
    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if(isDie==true)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
            Score_Manager.instance.IncreaseScore();
        }
    }

    private void OnDrawGizmos()
    {
        //��ȸ ������ �� �̵� ��� ǥ��
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination-transform.position);

        //��ǥ �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        //���� ����
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitiRange);

        //���� ����
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
