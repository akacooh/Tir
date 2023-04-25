using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour, IGameStateChangeResponder
{
    [SerializeField] private new Camera camera;
    [SerializeField] private float sensitivity;

    private float horizontal;
    private float vertical;

    void Update()
    {
        horizontal = Input.GetAxis("Mouse X") * sensitivity;
        transform.Rotate(0, horizontal, 0);

        vertical += Input.GetAxis("Mouse Y") * sensitivity;
        vertical = Mathf.Clamp(vertical, -80, 80);
        camera.transform.localRotation = Quaternion.Euler(-vertical, 0, 0);

    }

    public void ChangeSensitivity(float value) {
        sensitivity = value;
    }

    public void OnStateChanged(GameState state) {
        enabled = state switch {
            GameState.Pause => false,
            GameState.Start => true,
            GameState.Play => true,
            GameState.End => false,
            _ => throw new UnityException($"No behaviour for state {state}"),
        };
    }
}
