using UnityEngine;
using UnityEngine.UI;

// Score UI. Now a pure Observer: it knows nothing about enemies and is never
// referenced by them. It just listens for GameEvents.EnemyKilled and adds the
// score that the dying enemy published. The old Score_Manager.instance
// singleton and IncreaseScore() (which read EnemyFSM.instance.enemyScore) are
// gone, which removes both the global coupling and the score bug.
public class Score_Manager : MonoBehaviour
{
    public Text scoreText;
    private int totalScore = 0;

    private void OnEnable()
    {
        GameEvents.EnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= OnEnemyKilled; // required: the event is static
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    private void OnEnemyKilled(int score)
    {
        totalScore += score;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score : " + totalScore.ToString();
    }
}
