using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_Manager : MonoBehaviour
{
    public static Score_Manager instance;
    public Text scoreText;
    private EnemyFSM controller;
    private int totalScore = 0;

    private void Awake()
    {
        if(Score_Manager.instance == null)
        {
            Score_Manager.instance = this;
        }
        //controller=GetComponent<EnemyFSM>();

    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    public void IncreaseScore()
    {
        totalScore = totalScore + EnemyFSM.instance.enemyScore;
        UpdateScoreUI();    
    }
    void UpdateScoreUI()
    {
        scoreText.text = "Score : " + totalScore.ToString();
    }
}
