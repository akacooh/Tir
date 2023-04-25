using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    private int score;
    private float timeSinceStart;
    private float timeLeft;
    private GameState state;
    private GameState lastState;
    public UnityEvent<int> scoreChanged;
    public UnityEvent<GameState> stateChanged;
    
    [field: SerializeField] public float roundTime {private set; get;}
    [SerializeField] Shooting shootingScript;
    [SerializeField] Spawner spawnerScript;

    void Start()
    {
        StartGame();
        spawnerScript.newTargetCreated.AddListener(SubscribeTarget);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (state == GameState.Pause) {
                Resume();
            } else {
                ChangeState(GameState.Pause);
            }
        }
        if (state == GameState.Start) {
            timeSinceStart += Time.deltaTime;
            if (timeSinceStart >= 3.0f) {
                ChangeState(GameState.Play);
            }
        }

        if (state == GameState.Play) {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0) {
                ChangeState(GameState.End);
            }
        }
    }

    public void StartGame() {
        ChangeState(GameState.Start);
        timeSinceStart = 0;
        timeLeft = roundTime;
        score = 0;
        UpdateScore(0);
    }

    public void Resume() {
        ChangeState(lastState);
        timeSinceStart = 0;
        Debug.Log("Clicked");
    }

    private void ChangeState(GameState newState) {
        lastState = state;
        state = newState;
        stateChanged.Invoke(state);
    }

    private void UpdateScore(int value) {
        if (state == GameState.End) return;
        score += value;
        if (score < 0) score = 0;
        scoreChanged.Invoke(score);
    }

    private void SubscribeTarget(GameObject target) {
        TargetDummy targetScript = target.GetComponent<TargetDummy>();
        if (targetScript != null) {
            targetScript.died.AddListener(UpdateScore);
        }
    }
}

public enum GameState {
    Pause,
    Play,
    Start,
    End
}