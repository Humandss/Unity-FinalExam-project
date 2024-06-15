using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public GameObject esc_menu;
    public GameObject restart_pannel;

    private Player_Controller status;
    //public bool is_esc_menu_true = false;

    private void Awake()
    {
        status=GetComponent<Player_Controller>();
    }
    // Start is called before the first frame update
    void Start()
    {
        esc_menu.SetActive(false);
        restart_pannel.SetActive(false);
       // is_esc_menu_true = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape)) 
        { 
            esc_menu.SetActive(true);
           // is_esc_menu_true=true;
            Time.timeScale = 0f;
            
           
        }
    }
    public void ClickResumeButton()
    {
        esc_menu.SetActive(false);
       // is_esc_menu_true = false;
        Time.timeScale = 1f;
    }
    public void ClickMainMenuButton()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayerDie()
    {
       
        
      
    }
}
