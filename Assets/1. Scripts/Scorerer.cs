using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorerer : MonoBehaviour
{
    [SerializeField] float scoreMultiplier = 1f;
    ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = ScoreManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Runner")
        {
            var runner = other.GetComponentInParent<Runner>();
            scoreManager.AddScore(runner.ScoreToAdd * scoreMultiplier);
            runner.DestroyRunner();

        }
    }

}
