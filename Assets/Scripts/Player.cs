using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{

    [SerializeField] float moveSpeed = 5f;

    PlayerController controller;
    Camera viewCamera;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed; // Get the direction and multiply by speed.
        controller.Move(moveVelocity);

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.yellow);
            controller.LookAt(point);
        }
    }
}
