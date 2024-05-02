using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public float initialTimeInSeconds = 300; // Initial time for each player in seconds
    public TMPro.TMP_Text whiteClockText;
    public TMPro.TMP_Text blackClockText;

    public GameManager gameManager;

    private float whiteTimeRemaining;
    private float blackTimeRemaining;
    private bool isTimerRunning = false;


    void Start()
    {
        whiteTimeRemaining = initialTimeInSeconds;
        blackTimeRemaining = initialTimeInSeconds;
        
        InvokeRepeating("UpdateClock", 1f, 1f); // Update clock every second
    }

    void Update()
    {
        if (Board.gameStarted && !Board.gameEnded)
            isTimerRunning = true;

        if (isTimerRunning)
        {
            if (Board.isWhiteTurn)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                gameObject.transform.GetChild(1).gameObject.SetActive(false);
                whiteTimeRemaining -= Time.deltaTime;
            }
            else
            {
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                blackTimeRemaining -= Time.deltaTime;
            }

            if (whiteTimeRemaining <= 0 || blackTimeRemaining <= 0)
            {
                gameManager.EndGame(!Board.isWhiteTurn ? 1 : 2); // Time's up, end the game
            }
        }
    }

    void UpdateClock()
    {
        if (isTimerRunning)
        {
            if (Board.isWhiteTurn)
                whiteClockText.text = FormatTime(whiteTimeRemaining);
            else
                blackClockText.text = FormatTime(blackTimeRemaining);
        }
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
