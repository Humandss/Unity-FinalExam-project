using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHUD : MonoBehaviour
{
   
    
    private WeaponBase weapon;
    private Vector2 defaultWeaponIconSize;
    private Dictionary<Sprite, Sprite> croppedWeaponIconCache = new Dictionary<Sprite, Sprite>();

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
    [SerializeField]
    private float weaponIconScaleMultiplier = 1.6f;

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
        defaultWeaponIconSize = imageWeapon.rectTransform.sizeDelta;
        imageWeapon.preserveAspect = true;
        imageWeapon.rectTransform.localScale = Vector3.one;
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
        imageWeapon.sprite = GetDisplaySprite(spriteWeaponIcons[(int)weapon.WeaponName]);
        imageWeapon.preserveAspect = true;
        imageWeapon.rectTransform.localScale = Vector3.one;

        if (sizeWeaponIcons != null &&
            (int)weapon.WeaponName < sizeWeaponIcons.Length &&
            sizeWeaponIcons[(int)weapon.WeaponName] != Vector2.zero)
        {
            imageWeapon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName] * weaponIconScaleMultiplier;
        }
        else
        {
            imageWeapon.rectTransform.sizeDelta = defaultWeaponIconSize * weaponIconScaleMultiplier;
        }
    }
    private Sprite GetDisplaySprite(Sprite source)
    {
        if (source == null)
        {
            return null;
        }

        if (croppedWeaponIconCache.TryGetValue(source, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        try
        {
            Rect rect = source.rect;
            Texture2D texture = source.texture;
            Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

            int width = (int)rect.width;
            int height = (int)rect.height;
            int minX = width;
            int minY = height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (pixels[y * width + x].a <= 0.05f)
                    {
                        continue;
                    }

                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }

            if (maxX < minX || maxY < minY)
            {
                croppedWeaponIconCache[source] = source;
                return source;
            }

            Rect croppedRect = new Rect(rect.x + minX, rect.y + minY, maxX - minX + 1, maxY - minY + 1);
            Sprite croppedSprite = Sprite.Create(texture, croppedRect, new Vector2(0.5f, 0.5f), source.pixelsPerUnit);
            croppedSprite.name = source.name + "_Cropped";

            croppedWeaponIconCache[source] = croppedSprite;
            return croppedSprite;
        }
        catch
        {
            croppedWeaponIconCache[source] = source;
            return source;
        }
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

