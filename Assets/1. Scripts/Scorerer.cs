using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scorerer : MonoBehaviour
{
    [SerializeField] float scoreMultiplier = 1f;
    ScoreManager scoreManager;

    [SerializeField] Transform powerUpSpawnPoint;
    [SerializeField] List<GameObject> powerUps;
    [SerializeField] float scoreRequiredForPowerUp = 20;
    [SerializeField] PowerUp currentPowerUp;

    [SerializeField] Slider progressSlider;

    public float powerUpPercentage;
    float currentPowerUpScore;

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

            if(currentPowerUpScore < scoreRequiredForPowerUp)
            {
                currentPowerUpScore += 1;
                powerUpPercentage = currentPowerUpScore / scoreRequiredForPowerUp;
            }
            else
            {
                //powerup ready
                Debug.Log("PowerUpReady!");
                var powerUpObj = Instantiate(powerUps[Random.Range(0, powerUps.Count)], powerUpSpawnPoint.position, powerUpSpawnPoint.rotation, powerUpSpawnPoint);
                currentPowerUp = powerUpObj.GetComponent<PowerUp>();
                currentPowerUpScore = 0;
            }

            progressSlider.value = powerUpPercentage;
        }
    }

}
