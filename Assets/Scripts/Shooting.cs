using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Shooting : MonoBehaviour, IGameStateChangeResponder
{
    [SerializeField] new private Camera camera;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Text scoreText;
    [SerializeField] private Animator armsAnimator;
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private AudioClip shotAudio;

    public UnityEvent targetDestroyed;

    private float shotDistance = 50f;
    private float shotCooldown = 0.5f;
    private float lastShotTime;
    private AudioSource audioSource;

    void Start() {
        lastShotTime = Time.time;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetButton("Fire1") && (Time.time - lastShotTime) > shotCooldown) {
            lastShotTime = Time.time;
            Shoot();
            PlayAnimation();
            audioSource.Play();
        }
    }

    void Shoot() {
        Vector3 direction = Input.mousePosition - transform.position;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, shotDistance, mask)) {
            hit.collider.gameObject.GetComponent<TargetDummy>().OnShot();
            targetDestroyed.Invoke();
        }
    }

    void PlayAnimation() {
        armsAnimator.SetTrigger("Shot");
        gunAnimator.SetTrigger("Pulled");
    }
    
    public void OnStateChanged(GameState state) {
        enabled = state switch {
            GameState.Pause => false,
            GameState.Start => false,
            GameState.Play => true,
            GameState.End => false,
            _ => throw new UnityException($"No behaviour for state {state}"),
        };
    }
}
