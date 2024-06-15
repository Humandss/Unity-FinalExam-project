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
    private float targetRecognitionRange = 30; //인식 범위
    [SerializeField]
    private float pursuitLimitiRange = 50; // 추적 범위

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 20;
    [SerializeField]
    private float attackRate = 1;

    private EnemyState enemyState = EnemyState.None; //현재 적 행동 
    private float lastAttackTime = 0;   // 공격 주기 계산 변수

    private MovementStatus status; // 이동속도 등의 정보
    private NavMeshAgent navMeshAgent; // 이동 제어를 위한 네비게이션 메쉬
    private Transform target; //공격대상 => 플레이어
    private EnemyMemoryPool enemyMemoryPool; // 적 메모리 풀
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
    { //처음 적상태를 대기상태로 고정
        ChangeState(EnemyState.Idle);
    }
    private void OnDisable()
    {
        //적이 비활성화라면 현재 재생중인 상태로 종료, 상태를 none으로 설정
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }
    public void ChangeState(EnemyState newState)
    {
        //현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요없음
        if(enemyState == newState) return;
        //현재 재생중인 상태 종료
        StopCoroutine(enemyState.ToString());

        enemyState=newState;
        //새로운 상태재생
        StartCoroutine(enemyState.ToString());
    }
    private IEnumerator Idle()
    {
        //n초 후에 "배회"상태로 변경하는 코루틴
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            //타겟과의 거리에 따른 행동 선택
            CalculateDistanceToTargetAndSelectState();
            //대기 상태일때 하는 행동
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

        //이동 속도 설정
        navMeshAgent.speed = status.WalkSpeed;
        //목표 위치 설정 
        navMeshAgent.SetDestination(CalculateWanderPosition());

        //목표 위치로 회전 
        Vector3 to = new Vector3(navMeshAgent.destination.x,0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation=Quaternion.LookRotation(to-from);

        while(true)
        {
            currentTime += Time.deltaTime;

            //목표 위치에 근접하여 도달하거나 너무 오래시간동안 배회하기 상태에 머물러 있으면
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
        float wanderRadius = 10; // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0; // 선택된 각도
        int wanderJitterMin = 0; // 최소 각도
        int wanderJitterMax = 360; //최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기
        Vector3 rangerPosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        //자신의 위치를 중심으로 반지름 거리, 선택된 각도에 위치한 좌표를 목표 지점으로 설정
        wanderJitter= Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        //생성된 목표 위치가 자신의 이동 구역을 벗어나지 않게 조절
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
            //이동속도 설정
            navMeshAgent.speed = status.RunSpeed;
            // 목표 위치를 현재 플레이어 위치로 세팅
            navMeshAgent.SetDestination(target.position);
            // 타겟 방향을 주시
            LookRotationToTarget();
            // 타겟과의 거리에 따른 행동 선택
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
        
        
    }
    private void LookRotationToTarget()
    {
        //목표 위치
        Vector3 to = new Vector3(target.position.x,0, target.position.z);
        // 플레이어 위치
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
        //배회 상태일 때 이동 경로 표시
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination-transform.position);

        //목표 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        //추적 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitiRange);

        //공격 범위
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
