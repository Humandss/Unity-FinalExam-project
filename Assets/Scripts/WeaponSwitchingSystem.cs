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
        //���� ���� ����� ���� ���� �������� ��� ���� �̺�Ʈ ���
        playerHUD.SetUpAllWeapons(weapons);

        //���� �������� ������ �����ϰ� ������ �ʰԲ�
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
        //���� �Է� ������ ����Ī
        int inputIndex = 0;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5)) 
        {
            SwitchingWeapon((WeaponType)inputIndex-1);
        }

        
    }
    private void SwitchingWeapon(WeaponType weaponType)
    {
        //��ü ������ ���� ������ ����
        if (weapons[(int)weaponType]==null)
        {
            return;
        }

        //���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if(currentWeapon!=null)
        {
            previousWeapon= currentWeapon; 
        }
        //���� ��ü
        currentWeapon = weapons[(int)weaponType];

        //���� ������� ����� ��ü�ҷ� �Ѵٸ� ����
        if(currentWeapon==previousWeapon)
        {
            return;
        }
        //�ش� ���⸦ playercontroller�� playerhud�� ����
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        //,���� ����ϴ� ���� ��Ȱ��ȭ
        if(previousWeapon!=null)
        {
            previousWeapon.gameObject.SetActive(false);
        }
        //���� ����ϴ� ���� Ȱ��ȭ
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
