using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift; //달리기 키
    [SerializeField]
    private KeyCode keyCodeJump = KeyCode.Space; // 점프 키
    [SerializeField]
    private KeyCode keyCodeReload = KeyCode.R;//장전 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audiloClipWalk;
    [SerializeField]
    private AudioClip audioClipRun;

    public static Player_Controller instance;

    public float speed = 25.0f;
    public GameObject Bullets; 
    GameObject myInstance;
    public Transform FirePos;

    public bool while_aiming = false;

    public GameObject gun;

    private RotateCamera rotateToMouse; // 마우스 이동으로 카메라 회전
    private MovetmentCharacterController movement; // 키보드 입력으로 플레이어 이동, 점프
    private MovementStatus status;
    private AudioSource audioSource;
    private UI_Manager manager;
    private WeaponBase weapon; //무기 컨트롤러

    
    private void Awake()
    {
        if (Player_Controller.instance == null)
        {
            Player_Controller.instance = this;
        }
        
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            rotateToMouse = GetComponent<RotateCamera>();
            movement = GetComponent<MovetmentCharacterController>();
            status = GetComponent<MovementStatus>();
           // weapon = GetComponentInChildren<WeaponAssualt>();
            audioSource =GetComponent<AudioSource>();
            manager=GetComponent<UI_Manager>();
       
    }
    // Start is called before the first frame update
    void Start()
    {
       DrawWeapon();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();

    }
    private void UpdateWeaponAction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction();
        }
        if(Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }
        if(Input.GetMouseButtonDown(1))
        {
            weapon.StartWeaponAction(1);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            weapon.StopWeaponAction(1);
            
        }
    }
    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }
    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if( x!=0 || z !=0 ) // 이동 중일 때
        {
            bool isRun = false;

            //옆이나 뒤로 이동할 때는 달리기 x
            if (z > 0) 
            {
                isRun=Input.GetKey(keyCodeRun);
                
            }
            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            weapon.Animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audiloClipWalk;

            if(audioSource.isPlaying ==false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else // 제자리에 잇을 때
        {
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
            movement.MoveSpeed = 0;
            weapon.Animator.MoveSpeed = 0;

        }
        movement.MoveTo(new Vector3(x, 0, z));
    }
    private void UpdateJump()
    {
        if(Input.GetKeyDown(keyCodeJump)) 
        {
            movement.Jump();
        }
    }
   
    public void TakeDamage(int damage)
    {
        bool isDie =status.DecreaseHP(damage);

        if(isDie )
        {
            print("게임 오버");
            Time.timeScale = 0f;
            manager.PlayerDie();
        }
    }
    public void DrawWeapon()
    {
        gun.GetComponent<Animator>().Play("draw");
    }
    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }
   
}
