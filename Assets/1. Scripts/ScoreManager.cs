using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] float score;
    public static ScoreManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    public void AddScore(float amount)
    {
        //if(Gamemanager.GameRunning())
        score += amount;
        text.text = score.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        text.text = score.ToString();
    }

}
