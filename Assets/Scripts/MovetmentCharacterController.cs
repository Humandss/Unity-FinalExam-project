using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovetmentCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed; //이동속도
    private Vector3 moveForce; // 이동 힘 (x,z와 y축을 별도로 계싼해 실제 이동해 적용)

    [SerializeField]
    private float jumpForce; //점프 힘
    [SerializeField]
    private float gravity; // 중력 계수

    private CharacterController characterController; // 플레이어 이동 제어를 위한 컴포넌트
    
    
    // Start is called before the first frame update
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    public float MoveSpeed
    {
        set=>moveSpeed= Mathf.Max(0,value);
        get => moveSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        characterController.Move(moveForce*Time.deltaTime); // 1초당 moveForce 속력으로 이동

        if( !characterController.isGrounded )
        {
            moveForce.y += gravity * Time.deltaTime;
        }
    }
    public void MoveTo(Vector3 direction)
    {
        //이동 방향 = 캐릭터의 회전값 * 방향값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // 이동 힘 = 이동 방향* 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }
    public void Jump()
    {
        if(characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }
}
