using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHUD : MonoBehaviour
{
   
    
    private WeaponBase weapon;

    [SerializeField]
    private MovementStatus status;

    [Header("Weapon Base")]
    [SerializeField]
    private Text textWeaponName;
    [SerializeField]
    private Image imageWeapon;
    [SerializeField]
    private Sprite[] spriteWeaponIcons;
    [SerializeField]
    private Vector2[] sizeWeaponIcons; //무기마다 다른 아이콘의 적용

    public GameObject reloading_panel;

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab;
    [SerializeField]
    private Transform magazieParent;
    [SerializeField]
    private int maxMagazineCount; //처음 생성하는 탄창의 최대 갯수

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP; // 플레이어 체력을 나타냄
    [SerializeField]
    private Image imageBloodScreen; // 플레이어가 공격받았을 때 생기는 스크린
    [SerializeField]
    private AnimationCurve curveBloodScreen;
    [SerializeField]
    private Slider healthSlider;

    private List<GameObject> magazineList;

    private void Awake()
    {
      
        status.onHPEvent.AddListener(UpdatePHUD);
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
        reloading_panel.SetActive(false);

    }
    public void SetUpAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();

        for(int i=0; i<weapons.Length; ++i)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmo);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }
    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeapon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeapon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName]; 
    }
    private void UpdateAmmo(int currentAmmo, int maxAmmo) { 
    
     textAmmo.text= $"<size=40>{currentAmmo}/</size>{maxAmmo}";

        if (currentAmmo<= 0)
        {

            reloading_panel.SetActive(true);

        }
        else
        {
            reloading_panel.SetActive(false);
        }
    }
    private void SetupMagazine()
    {
        magazineList = new List<GameObject>();
        for(int i=0; i<maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazieParent);
            clone.SetActive(false) ;

            magazineList.Add(clone);

        }
      
    }
    private void UpdateMagazineHUD(int currentMagazine)
    {
        for(int i = 0; i<magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false) ;
        }
        for (int i = 0; i < currentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }
    private void UpdatePHUD(int previous, int current)
    {
        textHP.text = "HP" + current;
        healthSlider.value = ((float)current / 100);

        if (previous <= current) return;
        if (previous - current > 0) 
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }
    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while(percent<1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1,0,curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;


            yield return null;
        }
    }
    // Update is called once per frame
   public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon= newWeapon;
        SetupWeapon();
    }
  
}
