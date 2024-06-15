using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovetmentCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed; //�̵��ӵ�
    private Vector3 moveForce; // �̵� �� (x,z�� y���� ������ ����� ���� �̵��� ����)

    [SerializeField]
    private float jumpForce; //���� ��
    [SerializeField]
    private float gravity; // �߷� ���

    private CharacterController characterController; // �÷��̾� �̵� ��� ���� ������Ʈ
    
    
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
        characterController.Move(moveForce*Time.deltaTime); // 1�ʴ� moveForce �ӷ����� �̵�

        if( !characterController.isGrounded )
        {
            moveForce.y += gravity * Time.deltaTime;
        }
    }
    public void MoveTo(Vector3 direction)
    {
        //�̵� ���� = ĳ������ ȸ���� * ���Ⱚ
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // �̵� �� = �̵� ����* �ӵ�
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
