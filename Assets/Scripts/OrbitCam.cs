﻿using UnityEngine;

/// <summary>
/// Makes the camera orbit around the game object
/// </summary>
public class OrbitCam : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    /// <summary>
    /// The target to orbit
    /// </summary>
    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }
    
    [SerializeField]
    private float targetDistance = 5;
    /// <summary>
    /// The target distance between the target and the camera
    /// </summary>
    public float TargetDistance
    {
        get { return targetDistance; }
        set { targetDistance = value; }
    }

    /// <summary>
    /// The sensitivity of the mouse when using the camera
    /// </summary>
    [SerializeField]
    private float mouseSensitivity = 0.15f;

    /// <summary>
    /// The sensitivity of the mouse scroll wheel
    /// </summary>
    [SerializeField]
    private float scrollWheelSensitivity = 3f;

    /// <summary>
    /// The sensitivity of the mouse when using the camera
    /// </summary>
    [SerializeField]
    private float keyboardSensitivity = 0.15f;

    /// <summary>
    /// The direction from the camera to the target
    /// </summary>
    private Vector3 directionToTarget;
    
    /// <summary>
    /// The position of the mouse in the last frame
    /// </summary>
    private Vector2 mouseLastPosition;

    /// <summary>
    /// The x angle to orbit the object at
    /// </summary>
    float angleX = 0;

    /// <summary>
    /// The y angle to orbit the object at
    /// </summary>
    float angleY = 0;

    void Awake()
    {
        directionToTarget = transform.forward;
    }

    void Update()
    {
        // Get new rotation values
        // Check user input (Look)
        if (Input.GetMouseButton(1))
        {
            // Get the difference mouse drag
            Vector2 mouseDrag = (Vector2)Input.mousePosition - mouseLastPosition;

            // Get rotation and multiply by sensitivity
            Vector2 newRotation = Vector2.zero;
            angleX += -mouseDrag.x * mouseSensitivity * Time.deltaTime;
            angleY += mouseDrag.y * mouseSensitivity * Time.deltaTime * -1;
        }
        else if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.K))
        {
            // LEFT
            if (Input.GetKey(KeyCode.J))
            {
                angleX += -1 * keyboardSensitivity * Time.deltaTime;
            }

            // UP
            if (Input.GetKey(KeyCode.I))
            {
                angleY += -1 * keyboardSensitivity * Time.deltaTime;
            }

            // RIGHT
            if (Input.GetKey(KeyCode.L))
            {
                angleX += 1 * keyboardSensitivity * Time.deltaTime;
            }

            // DOWN
            if (Input.GetKey(KeyCode.K))
            {
                angleY += 1 * keyboardSensitivity * Time.deltaTime;
            }
        }

        // Apply scrolling
        targetDistance += (Input.GetAxis("Mouse ScrollWheel") * scrollWheelSensitivity);
        targetDistance = Mathf.Clamp(targetDistance, 2, 100);

        // Perform rotation (SOURCE: https://forum.unity.com/threads/follow-orbit-camera.202490/ -- By user "WorldArchitect", Post #6)
        float x = targetDistance * Mathf.Cos(angleX) * Mathf.Sin(angleY);
        float z = targetDistance * Mathf.Sin(angleX) * Mathf.Sin(angleY);
        float y = targetDistance * Mathf.Cos(angleY);
        transform.position = new Vector3(x + target.transform.position.x,
                                         y + target.transform.position.y,
                                         z + target.transform.position.z);

        // Look at the bus
        transform.LookAt(target.transform.position);
        
        // Update the mouse last position
        mouseLastPosition = Input.mousePosition;
    }
}