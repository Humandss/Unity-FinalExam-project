using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManger : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
  
    }
    public void GameRestart()
    {

        SceneManager.LoadSceneAsync(0);
        Player.Instance.player_HP = 100;
        Time.timeScale = 1f;
        
    }
    
}
