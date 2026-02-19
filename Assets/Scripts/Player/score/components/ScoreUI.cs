using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI totalScore;

    private Score score;

    private void Awake()
    {
        score = GetComponent<Score>();
    }

    public void ConnectScoreUI(Score score)
    {
        this.score = score;
        score.OnScoreChange += UpdateScore;
    }

    private void OnDestroy()
    {
        score.OnScoreChange -= UpdateScore;
    }

    private void UpdateScore(int score)
    {
        if(totalScore != null)
            totalScore.text = score.ToString();
    }

}
