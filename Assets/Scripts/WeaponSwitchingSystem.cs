using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchingSystem : MonoBehaviour
{
    [SerializeField]
    private Player_Controller playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponBase[] weapons;

    private WeaponBase currentWeapon;
    private WeaponBase previousWeapon;

    private void Awake()
    {
        //무기 정보 출력을 위해 현재 소지중인 모든 무기 이벤트 등록
        playerHUD.SetUpAllWeapons(weapons);

        //현재 소지중인 아이템 제외하고 보이지 않게끔
        for(int i=0; i<weapons.Length; ++i)
        {
            if (weapons[i].gameObject !=null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
        SwitchingWeapon(WeaponType.Main);
    }
    private void Update()
    {
        UpdateSwitch();
    }
    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;
        //숫자 입력 받을시 스위칭
        int inputIndex = 0;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5)) 
        {
            SwitchingWeapon((WeaponType)inputIndex-1);
        }

        
    }
    private void SwitchingWeapon(WeaponType weaponType)
    {
        //교체 가능한 무기 없으면 종료
        if (weapons[(int)weaponType]==null)
        {
            return;
        }

        //현재 사용중인 무기가 있으면 이전 무기 정보에 저장
        if(currentWeapon!=null)
        {
            previousWeapon= currentWeapon; 
        }
        //무기 교체
        currentWeapon = weapons[(int)weaponType];

        //현재 사용중인 무기로 교체할려 한다면 종료
        if(currentWeapon==previousWeapon)
        {
            return;
        }
        //해당 무기를 playercontroller와 playerhud에 전달
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        //,현재 사용하던 무기 비활성화
        if(previousWeapon!=null)
        {
            previousWeapon.gameObject.SetActive(false);
        }
        //현재 사용하는 무기 활성화
        currentWeapon.gameObject.SetActive(true);
    }

    public void IncreaseMagazine(WeaponType weaponType, int magazine)
    {
        if (weapons[(int)weaponType] != null)
        {
            weapons[(int)weaponType].IncreaseMagazine(magazine);
        }
    }

    public void IncreaseMagazine(int magazine)
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i] != null)
            {
                weapons[i].IncreaseMagazine(magazine);
            }
        }
    }
}
