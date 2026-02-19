using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    private List<Score> scores = new List<Score>();

    [SerializeField]
    private Transform scoreUIPrefab;

    [SerializeField]
    private Transform scoreUIParent;

    private void Start()
    {
        //ToggleAllScoresVisible();
    }

    private void CreateScoreUI(Score score)
    {
        var scoreUI = Instantiate(scoreUIPrefab, scoreUIParent);
        scoreUI.GetComponent<ScoreUI>().ConnectScoreUI(score);
    }

    public void AddPlayer(Score score)
    {
        scores.Add(score);
        CreateScoreUI(score);
    }

    public void ResetScores()
    {
        foreach (var score in scores)
            score.ResetScore();
    }

    public void ToggleAllScoresVisible()
    {
        scoreUIParent.gameObject.SetActive(!scoreUIParent.gameObject.activeSelf);
    }

}
