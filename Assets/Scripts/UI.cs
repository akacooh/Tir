using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI : MonoBehaviour, IGameStateChangeResponder
{
    [SerializeField] private Game gameScript;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private Text endScore;

    [Tooltip("Object for countdown at start")]
    [SerializeField] private GameObject timer;

    private bool enableTimer;
    private float timeLeft;

    public void OnStateChanged(GameState state)
    {
        //pause
        if (state == GameState.Pause) {
            Cursor.lockState = CursorLockMode.None;
            enableTimer = false;
            StopCountDown();
            menu.SetActive(true);
            endScreen.SetActive(false);

        // normal
        } else if (state == GameState.Play) {
            Cursor.lockState = CursorLockMode.Locked;
            StopCountDown();
            enableTimer = true;
            menu.SetActive(false);

        //start
        } else if (state == GameState.Start) {
            Cursor.lockState = CursorLockMode.Locked;
            timeLeft = gameScript.roundTime;
            StartCountDown();
            menu.SetActive(false);
            endScreen.SetActive(false);

        //end game
        } else if (state == GameState.End) {
            Cursor.lockState = CursorLockMode.None;
            enableTimer = false;
            menu.SetActive(false);
            endScreen.SetActive(true);
            endScore.text = "Your " + scoreText.text;
        } else {
            throw new UnityException($"No behaviour for state {state}");
        }
    }

    void Start()
    {
        gameScript.scoreChanged.AddListener(UpdateScore);
    }

    void Update()
    {
        if(enableTimer) {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) timeLeft = 0;
            timerText.text = "Time left: " + timeLeft.ToString("F1");
        }
    }

    void UpdateScore(int value) {
        scoreText.text = "Score: " + value;
    }

    private void StartCountDown() {
        timer.SetActive(true);
    }

    private void StopCountDown() {
        timer.SetActive(false);
    }


}
