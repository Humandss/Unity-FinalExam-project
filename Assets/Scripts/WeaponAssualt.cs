using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAssualt : WeaponBase
{
  
    [Header("Aim UI")]
    [SerializeField]
    private Image imageAim;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; //무기 장착 사운드
    [SerializeField] 
    private AudioClip audioClipFire; //발사 사운드
    [SerializeField]
    private AudioClip audioClipReload;

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; // 총구 이펙트

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoints; //탄피 이펙트

    [SerializeField]
    private Transform bulletSpawnPoint; //총알 이펙트

    private CasingMemoryPool casingMemoryPool;
    private ImpactMemoryPool impactMemoryPool;
    private Camera mainCamera;

    private bool isModeChange = false; // 모든 전환 여부
    private float defaultModeFOV = 60; //기본 fov값
    private float aimModeFOV = 30; // aim모드시 fov값
   

    private void Awake()
    {
        base.Setup();
        casingMemoryPool=GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera=Camera.main;

        weaponSetting.currentAmmo = weaponSetting.maxAmmo; //현재 탄수를 최대로 지정
        weaponSetting.CurrentMagazine = weaponSetting.maxMagazine; //처음 탄창 수 최대로 지정

    }
    private void OnEnable()
    {
        muzzleFlashEffect.SetActive(false); // 총구 이펙트 비활성화
        PlaySound(audioClipTakeOutWeapon);

        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        onMagazineEvent.Invoke(weaponSetting.CurrentMagazine);

        ResetVariables();
    }
    public override void StartWeaponAction(int type = 0)
    {
        if (isReload == true) return; //재장전중일 때 공격x

        if (isModeChange == true) return; // 모드 전환일 때 공격x

        if (type == 0)
        {
            if (weaponSetting.isAutomaticFire == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            else
            {
                OnAttack();
            }
        }
        else
        {
            if (isAttack == true) return;

            StartCoroutine("OnModeChange");
        }
    }
    public override void StopWeaponAction(int type=0) 
    { if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
            
                }
    public override void StartReload()
    {
        if (isReload == true || weaponSetting.CurrentMagazine <= 0)
        {
            return;
        }
        StopWeaponAction();

        StartCoroutine("OnReload");
    }
    private IEnumerator OnAttackLoop()
    {
        while(true) 
        {
            OnAttack();
            yield return null;
        }
    }
    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.fireRate) 
        { 
          lastAttackTime = Time.time;
            //탄수가 없으면 공격 불가
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            //공격시 탄 감소
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

           // if (animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

            StartCoroutine("OnMuzzleFlashEffect");
            PlaySound(audioClipFire);

           
            animator.Play("Fire", -1, 0);
           
            
            casingMemoryPool.SpawnCasing(casingSpawnPoints.position, transform.right); //탄피 생성

            TwoStepRaycast(); // 총알(광선) 발사
        }
    }
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.fireRate * 2f);

        muzzleFlashEffect.SetActive(false);
    }
    private IEnumerator OnReload()
    {
        isReload = true;

        
        animator.OnReload();   
        PlaySound(audioClipReload);

        while(true)
        {
            if (audioSource.isPlaying == false && (animator.CurrentAnimationIs("New State")|| animator.CurrentAnimationIs("Aiming")))
            {
                isReload = false;
                weaponSetting.CurrentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.CurrentMagazine);

                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }
            yield return null;
        }
    }
    
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        //화면의 중앙 좌표(aim 기준으로 위치 계산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        //사거리안에 부딪히는 오브젝트 좦 계싼
        if (Physics.Raycast(ray, out hit, weaponSetting.fireDistance))
        {
            targetPoint=hit.point;
        }
        // 최대 사거리에 오브젝트 없으면 targetpoint를 최대 사거리에 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.fireDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction*weaponSetting.fireDistance, Color.black);

        //첫번째 레이케스트 여산으로 입력된 targetpoint를 목표지정으로 설정하고,
        // 총구를 시작지점으로 하여 raycast 연산
        Vector3 fireDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, fireDirection, out hit, weaponSetting.fireDistance))
        {
            impactMemoryPool.SpawnImpact(hit);
            if(hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if(hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, fireDirection*weaponSetting.fireDistance, Color.blue);
    }
    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        animator.AimModeIs=!animator.AimModeIs;
       // imageAim.enabled=!imageAim.enabled;

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        isModeChange = true;

        while(percent<1)
        { 
        
           current+=Time.deltaTime;
            percent = current / time;

            mainCamera.fieldOfView=Mathf.Lerp(start,end,percent);

            yield return null;
        
        }
        isModeChange = false;
    }
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }

   /* public void IncreaseMagazine(int magazine)
    {
        weaponSetting.CurrentMagazine = CurrentMagazine + magazine > MaxMagazine ? MaxMagazine : CurrentMagazine + magazine;

        onMagazineEvent.Invoke(CurrentMagazine);
    }*/
    
  
}
