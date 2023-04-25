using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetDummy : MonoBehaviour
{
    [SerializeField] private float timeToLive;
    [SerializeField] private ParticleSystem explosion;

    private bool wasKilled = false;
    public UnityEvent<int> died;
    private float timeAlive = 0;

    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= timeToLive) {
            DisableObject();
        }
    }

    private void OnEnable() {
        explosion.Stop();
        wasKilled = false;
    }

    private void DisableObject() {
        gameObject.SetActive(false);
    }

    //reset object if it was "destroyed" (disabled)
    private void OnDisable() {
        if (gameObject.activeSelf) return;
        timeAlive = 0;
        if (wasKilled) {
            died.Invoke(1);
            explosion.Play();
        } else {
            died.Invoke(-1);
        }
    }

    public void OnShot() {
        wasKilled = true;
        DisableObject();
    }
}
