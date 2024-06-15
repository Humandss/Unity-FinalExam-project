using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField]
    private float rotCamXAxisSpeed = 2; //ī�޶� x�� ȸ�� �ӵ�
    [SerializeField]
    private float rotCamYAxisSpeed = 2; // ī�޶� y�� ȸ�� �ӵ�

    private float limitMinX = -80; // ī�޶� x�� ȸ�� ����(�ּ�)
    private float limitMinY = 50; // ī�޶� x�� ȸ�� ����(�ִ�)

    private float eulerAngleX;
    private float eulerAngleY;

    public void UpdateRotate(float mouseX, float mouseY)
    {
        if (Time.timeScale == 1) //������ �������̶�� 
        {
            eulerAngleY += mouseX * rotCamYAxisSpeed; //���콺 ��/�� �̵����� ī�޶� y�� ȸ��
            eulerAngleX -= mouseY * rotCamXAxisSpeed; // ���콺 ��/�Ʒ� �̵����� ī�޶� x�� ȸ��

            //ī�޶� x�� ȸ���� ��� ȸ�� ������ ����
            eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMinY);

            transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);

        }
        else if(Time.timeScale == 0) //������ �Ͻ� ���� �� ���¶��
        {
            Cursor.visible = true; //�ٽ� Ŀ���� ���
            Cursor.lockState = CursorLockMode.None;
        }
       
    }
   private float ClampAngle(float angle, float min, float max)
    {
        if(angle < -360) 
        {
            angle += 360;
        }
        if(angle >360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
    
}
