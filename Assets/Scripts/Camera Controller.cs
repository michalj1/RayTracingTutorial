using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    float rotationSpeed = 10.0f;
    float moveSpeed = 0.2f;

    // Update is called once per frame
    void Update() {
        // Get inputs
        float sidewaysMovement = Input.GetAxis("Horizontal");
        float forwardMovement = Input.GetAxis("Vertical");
        float horizontalRotation = Input.GetAxis("Mouse X");
        float verticalRotation = Input.GetAxis("Mouse Y");
        Transform transform = Camera.main.transform;

        // Rotate camera
        Vector3 rotation = horizontalRotation * new Vector3(0, 1, 0) + -verticalRotation * new Vector3(1, 0, 0);
        transform.eulerAngles = transform.eulerAngles + rotationSpeed * rotation;

        // Move camera
        Vector3 movement = forwardMovement * transform.forward + sidewaysMovement * transform.right;
        transform.Translate(moveSpeed * movement);

    }
}
