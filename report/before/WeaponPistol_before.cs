using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPistol : WeaponBase
{
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect;
    [Header("Spawn Points")]
    [SerializeField]
    private Transform bulletSpawnPoint;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipFire;
    [SerializeField]
    private AudioClip audioClipReload;

    private ImpactMemoryPool impactMemoryPool;
    private Camera mainCamera;

    private void Awake()
    {
        base.Setup();

        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        weaponSetting.CurrentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;

    }
    private void OnEnable()
    {
        muzzleFlashEffect.SetActive(false);

        onMagazineEvent.Invoke(weaponSetting.CurrentMagazine);

        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }
    
    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && isAttack == false && isReload == false) 
        {
            OnAttack();
        }
    }
    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
    }
    public override void StartReload()
    {
        if (isReload == true || weaponSetting.CurrentMagazine <= 0) return;

        StopWeaponAction();

        StartCoroutine("OnReload");
    }
    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.fireRate)
        {
            //뛰고 있을 때 공격 불가
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            lastAttackTime=Time.time;

            if(weaponSetting.currentAmmo<=0)
            {
                return;
            }
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            StartCoroutine("OnMuzzleFlashEffect");
            PlaySound(audioClipFire);


            animator.Play("Fire", -1, 0);

            TwoStepRaycast();
        }
    }
    private IEnumerator OnMuzzleFlashEffect()
    {

        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.fireRate * 1f);

        muzzleFlashEffect.SetActive(false);

    }
    private IEnumerator OnReload()
    {

        isReload = true;


        animator.OnReload();
        PlaySound(audioClipReload);

        while (true)
        {
            if (audioSource.isPlaying == false && (animator.CurrentAnimationIs("New State") || animator.CurrentAnimationIs("Aiming")))
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
            targetPoint = hit.point;
        }
        // 최대 사거리에 오브젝트 없으면 targetpoint를 최대 사거리에 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.fireDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.fireDistance, Color.black);

        //첫번째 레이케스트 여산으로 입력된 targetpoint를 목표지정으로 설정하고,
        // 총구를 시작지점으로 하여 raycast 연산
        Vector3 fireDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, fireDirection, out hit, weaponSetting.fireDistance))
        {
            impactMemoryPool.SpawnImpact(hit);
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, fireDirection * weaponSetting.fireDistance, Color.blue);
    }

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
    }

}
