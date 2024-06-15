using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance;
    public int player_HP = 100;

    public GameObject restartPanel;

    private void Awake()
    {
        if (Player.Instance == null)
        {
            Player.Instance = this;
        }


    }


    // Start is called before the first frame update
    void Start()
    {
       restartPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Player_Die();
    }
   
    void Player_Die() //�÷��̾� ����� ȣ��Ǵ� �Լ�
    {
        if (player_HP == 0)
        {
            print("�÷��̾� ���!");
            restartPanel.SetActive(true);

            Time.timeScale = 0f;

        }
    }
    private void OnTriggerEnter(Collider other) //���� �ε����� �� �浹 ����
    {
        if (other.CompareTag("Enemy"))
        {
            player_HP = player_HP - 10;
        }
    }
}

