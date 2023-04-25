using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IGameStateChangeResponder
{

    [SerializeField] private float speed;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private LayerMask wallMask;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0, z);
        direction = transform.rotation * direction.normalized * speed * Time.deltaTime;

        Move(direction);
        
    }

    void Move(Vector3 direction)
    {
        Ray ray = new Ray(rayOrigin.position, direction);
        RaycastHit hit;

        //0.01 offset for sphere to not appear in wall
        if (!Physics.SphereCast(ray, 0.5f, out hit, direction.magnitude + 0.01f, wallMask)) {
            //Debug.DrawRay(rayOrigin.position, direction.normalized * 5, Color.green, 0.5f);
            transform.Translate(direction, Space.World);
        }
        else {
            //Debug.DrawRay(rayOrigin.position, direction.normalized * 5, Color.red, 0.5f);
            //Debug.DrawRay(hit.point, hit.normal.normalized * 5, Color.blue, 0.5f);

            Vector3 slideDirection = Vector3.ProjectOnPlane(direction, hit.normal);
            Ray slideRay = new Ray(rayOrigin.position, slideDirection);
            if (!Physics.SphereCast(slideRay, 0.5f, slideDirection.magnitude + 0.01f, wallMask)) {
                transform.Translate(slideDirection, Space.World);
                //Debug.DrawRay(rayOrigin.position + hit.normal * 0.01f, slideDirection.normalized * 5, Color.blue, 0.5f);    
            }
        }
    }

    public void OnStateChanged(GameState state) {
        enabled = state switch {
            GameState.Pause => false,
            GameState.Start => false,
            GameState.Play => true,
            GameState.End => false,
            _ => throw new UnityException($"No behaviour for state {state}"),
        };

        if (state == GameState.Start) {
            transform.position = new Vector3(0, 0.14f, -5.75f); //original position
        }
    }
}
