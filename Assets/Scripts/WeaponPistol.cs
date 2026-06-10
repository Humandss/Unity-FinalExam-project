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
            //�ٰ� ���� �� ���� �Ұ�
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

        //ȭ���� �߾� ��ǥ(aim �������� ��ġ ���)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        //��Ÿ��ȿ� �ε����� ������Ʈ �B ���
        if (Physics.Raycast(ray, out hit, weaponSetting.fireDistance))
        {
            targetPoint = hit.point;
        }
        // �ִ� ��Ÿ��� ������Ʈ ������ targetpoint�� �ִ� ��Ÿ��� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.fireDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.fireDistance, Color.black);

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
        Debug.DrawRay(bulletSpawnPoint.position, fireDirection * weaponSetting.fireDistance, Color.blue);
    }

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
    }

}
