using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Main=0, Sub, Throw}

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int,int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public abstract class WeaponBase : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("WeaponBase")]
    [SerializeField]
    protected WeaponType weaponType; //무기 종류
    [SerializeField]
    protected WeaponSetting weaponSetting;

    protected float lastAttackTime = 0;
    protected bool isReload = false;
    protected bool isAttack = false;
    protected AudioSource audioSource;
    protected PlayerAnimatorController animator;

    //외부에서 이벤트 함수 등록을 할 수 있도록 선언
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagazineEvent=new MagazineEvent();

    //외부에서 필요한 정보를 열람하기 위한 프로퍼티
    public PlayerAnimatorController Animator=>animator;
    public WeaponName WeaponName=>weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.CurrentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    public abstract void StartWeaponAction(int type=0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    protected void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
    protected void Setup()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<PlayerAnimatorController>();
    }
    public virtual void IncreaseMagazine(int magazine)
    {
        weaponSetting.CurrentMagazine = CurrentMagazine + magazine > MaxMagazine ? MaxMagazine : CurrentMagazine + magazine;

        onMagazineEvent.Invoke(CurrentMagazine);
    }
}
