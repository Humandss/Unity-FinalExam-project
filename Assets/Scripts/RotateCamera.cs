using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField]
    private float rotCamXAxisSpeed = 2; //카메라 x축 회전 속도
    [SerializeField]
    private float rotCamYAxisSpeed = 2; // 카메라 y축 회전 속도

    private float limitMinX = -80; // 카메라 x축 회전 범위(최소)
    private float limitMinY = 50; // 카메라 x축 회전 범위(최대)

    private float eulerAngleX;
    private float eulerAngleY;

    public void UpdateRotate(float mouseX, float mouseY)
    {
        if (Time.timeScale == 1) //게임이 실행중이라면 
        {
            eulerAngleY += mouseX * rotCamYAxisSpeed; //마우스 좌/우 이동으로 카메라 y축 회전
            eulerAngleX -= mouseY * rotCamXAxisSpeed; // 마우스 위/아래 이동으로 카메라 x축 회전

            //카메라 x축 회전의 경우 회전 범위를 설정
            eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMinY);

            transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);

        }
        else if(Time.timeScale == 0) //게임이 일시 정지 된 상태라면
        {
            Cursor.visible = true; //다시 커서를 띄움
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
