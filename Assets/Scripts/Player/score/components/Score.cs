using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{

    private int pointTotal;

    [SerializeField]
    private int startingPoints;


    public delegate void ScoreChangedCallback(int score);
    public event ScoreChangedCallback OnScoreChange;


    public int PointTotal { get => pointTotal; }

    private void Awake()
    {
    }

    public void AddToScore(int value)
    {
        pointTotal += value;

        if (OnScoreChange != null)
            OnScoreChange(pointTotal);
    }

    public bool SpendScore(int cost)
    {
        var canBuy = false;
        if(cost <= pointTotal)
        {
            canBuy = true;
            pointTotal -= cost;

            if (OnScoreChange != null)
                OnScoreChange(pointTotal);
        }

        return canBuy;
    }

    public bool CheckForEnoughPoints(int points)
    {
        return points < pointTotal;
    }

    public void ResetScore()
    {
        pointTotal = startingPoints;

        if (OnScoreChange != null)
            OnScoreChange(pointTotal);
    }
}
