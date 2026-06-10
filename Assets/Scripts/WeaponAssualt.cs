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
    private AudioClip audioClipTakeOutWeapon; //���� ���� ����
    [SerializeField] 
    private AudioClip audioClipFire; //�߻� ����
    [SerializeField]
    private AudioClip audioClipReload;

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; // �ѱ� ����Ʈ

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoints; //ź�� ����Ʈ

    [SerializeField]
    private Transform bulletSpawnPoint; //�Ѿ� ����Ʈ

    private CasingMemoryPool casingMemoryPool;
    private ImpactMemoryPool impactMemoryPool;
    private Camera mainCamera;

    private bool isModeChange = false; // ��� ��ȯ ����
    private float defaultModeFOV = 60; //�⺻ fov��
    private float aimModeFOV = 30; // aim���� fov��
   

    private void Awake()
    {
        base.Setup();
        casingMemoryPool=GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera=Camera.main;

        weaponSetting.currentAmmo = weaponSetting.maxAmmo; //���� ź���� �ִ�� ����
        weaponSetting.CurrentMagazine = weaponSetting.maxMagazine; //ó�� źâ �� �ִ�� ����

    }
    private void OnEnable()
    {
        muzzleFlashEffect.SetActive(false); // �ѱ� ����Ʈ ��Ȱ��ȭ
        PlaySound(audioClipTakeOutWeapon);

        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        onMagazineEvent.Invoke(weaponSetting.CurrentMagazine);

        ResetVariables();
    }
    public override void StartWeaponAction(int type = 0)
    {
        if (isReload == true) return; //���������� �� ����x

        if (isModeChange == true) return; // ��� ��ȯ�� �� ����x

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
            //ź���� ������ ���� �Ұ�
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            //���ݽ� ź ����
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

           // if (animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

            StartCoroutine("OnMuzzleFlashEffect");
            PlaySound(audioClipFire);

           
            animator.Play("Fire", -1, 0);
           
            
            casingMemoryPool.SpawnCasing(casingSpawnPoints.position, transform.right); //ź�� ����

            TwoStepRaycast(); // �Ѿ�(����) �߻�
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

        //ȭ���� �߾� ��ǥ(aim �������� ��ġ ���)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        //��Ÿ��ȿ� �ε����� ������Ʈ �B ���
        if (Physics.Raycast(ray, out hit, weaponSetting.fireDistance))
        {
            targetPoint=hit.point;
        }
        // �ִ� ��Ÿ��� ������Ʈ ������ targetpoint�� �ִ� ��Ÿ��� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.fireDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction*weaponSetting.fireDistance, Color.black);

        //ù��° �����ɽ�Ʈ �������� �Էµ� targetpoint�� ��ǥ�������� �����ϰ�,
        // �ѱ��� ������������ �Ͽ� raycast ����
        Vector3 fireDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, fireDirection, out hit, weaponSetting.fireDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(weaponSetting.damage);
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
