using UnityEngine;

/**
 * FlyCam allows the user to control the in-game camera like the editor camera.
 */
public class FlyCam : MonoBehaviour {

    // Acceleration: The acceleration factor added to the speed
    public float acceleration;

    // FastMultipier: How much to multiply the current speed by WHEN GOING FAST
    public float fastMultiplier;

    // MaxSpeed: The maximum speed
    public float maxSpeed;

    // MaxSlowSpeed: The max speed when moving slowly
    public float maxSlowSpeed;

    // MouseSensitivity: Self explanatory
    public float mouseSensitivity;

    // Speed: The camera's current speed
    private float _speed;

    // MovementVector: The direction that the camera is moving
    private Vector3 _movementVector;

    // The last position of the mouse
    private Vector2 _mouseLastPosition;

	/**
     * Update checks for key inputs and delegates to movement methods.
     */
	private void Update () {

        // Initialise if the FlyCam is moving to 0
        bool isMoving = false;

        // Create an empty movement vector
        _movementVector = new Vector3(0, 0, 0);

        // Check user input (Left)
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            isMoving = true;
            _movementVector += Vector3.left;
        }

        // Check user input (Forward)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            isMoving = true;
            _movementVector += Vector3.forward;
        }

        // Check user input (Right)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            isMoving = true;
            _movementVector += Vector3.right;
        }

        // Check user input (Backward)
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            isMoving = true;
            _movementVector += Vector3.back;
        }

        // Check user input (Up)
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.PageUp))
        {
            isMoving = true;
            _movementVector += Vector3.up;
        }

        // Check user input (Down)
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.PageDown))
        {
            isMoving = true;
            _movementVector += Vector3.down;
        }

        // Check user input (Look)
        if (Input.GetMouseButton(1))
        {
            // Get the difference mouse drag
            Vector2 mouseDrag = (Vector2)Input.mousePosition - _mouseLastPosition;

            // Get rotation and multiply by sensitivity
            Vector2 newRotation = Vector2.zero;
            newRotation.y = mouseDrag.x * mouseSensitivity;
            newRotation.x = -(mouseDrag.y) * mouseSensitivity;

            // Apply rotation
            transform.eulerAngles += (Vector3)newRotation;
        }

        // Accelerate
        _speed += acceleration;
        _speed = Mathf.Min(_speed, maxSpeed);

        // Move the FlyCam, stop the speed if the user is not moving
        if (isMoving)
        {
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _speed = _speed * fastMultiplier;
                gameObject.transform.Translate(_movementVector * _speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                _speed = Mathf.Min(_speed, maxSlowSpeed);
                gameObject.transform.Translate(_movementVector * _speed * Time.deltaTime);
            }
            else
            {
                gameObject.transform.Translate(_movementVector * _speed * Time.deltaTime);
            }
        }
        else
        {
            _speed = 0;
        }

        // Update the mouse last position
        _mouseLastPosition = Input.mousePosition;
    }
}
