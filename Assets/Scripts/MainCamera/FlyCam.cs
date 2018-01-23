using AaronMeaney.InputManagement;
using AaronMeaney.BusStop.InputManagement;
using UnityEngine;

namespace AaronMeaney.BusStop.MainCamera
{
    /// <summary>
    /// FlyCam allows the user to control the in-game camera like the editor camera.
    /// </summary>
    public class FlyCam : MonoBehaviour
    {
        private InputManager _inputManager;

        [SerializeField]
        public float acceleration;
        /// <summary>
        /// The acceleration factor added to the squad.
        /// </summary>
        public float Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        [SerializeField]
        private float fastMultiplier;
        /// <summary>
        /// How much to multiply the current speed by, when GOING FAST.
        /// </summary>
        public float FastMultiplier
        {
            get { return fastMultiplier; }
            set { fastMultiplier = value; }
        }

        [SerializeField]
        public float maxSpeed;
        /// <summary>
        /// The maximum speed when going normally.
        /// </summary>
        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }

        [SerializeField]
        public float maxSlowSpeed;
        /// <summary>
        /// The maximum speed when going slowly.
        /// </summary>
        public float MaxSlowSpeed
        {
            get { return maxSlowSpeed; }
            set { maxSlowSpeed = value; }
        }

        /// <summary>
        /// The sensitivity of the mouse when using the camera.
        /// </summary>
        [SerializeField]
        private float mouseSensitivity;

        /// <summary>
        /// The current speed.
        /// </summary>
        [SerializeField]
        private float speed;

        /// <summary>
        /// The current movement vector of the camera.
        /// </summary>
        private Vector3 movementVector;

        /// <summary>
        /// The last position of the mouse.
        /// </summary>
        private Vector2 mouseLastPosition;
        
        private void Awake()
        {
            _inputManager = FindObjectOfType<InputManager>();
        }

        /// <summary>
        /// Update checks for key inputs and delegates to movement methods.
        /// </summary>
        private void Update()
        {
            // Initialise if the FlyCam is moving to 0
            bool isMoving = false;

            // Create an empty movement vector
            movementVector = new Vector3(0, 0, 0);
            
            // Check user inputs
            if (_inputManager.IsActionPressed<FlyCamMoveLeftAction>())
            {
                isMoving = true;
                movementVector += Vector3.left;
            }
            
            if (_inputManager.IsActionPressed<FlyCamMoveForwardAction>())
            {
                isMoving = true;
                movementVector += Vector3.forward;
            }
            
            if (_inputManager.IsActionPressed<FlyCamMoveRightAction>())
            {
                isMoving = true;
                movementVector += Vector3.right;
            }
            
            if (_inputManager.IsActionPressed<FlyCamMoveBackwardAction>())
            {
                isMoving = true;
                movementVector += Vector3.back;
            }
            
            if (_inputManager.IsActionPressed<FlyCamMoveUpAction>())
            {
                isMoving = true;
                movementVector += Vector3.up;
            }
            
            if (_inputManager.IsActionPressed<FlyCamMoveDownAction>())
            {
                isMoving = true;
                movementVector += Vector3.down;
            }

            // Initialise rotation
            Vector2 rotationChange = Vector2.zero;

            // Check user input (Look)
            if (Input.GetMouseButton(1))
            {
                // Get the difference mouse drag
                Vector2 mouseDrag = (Vector2)Input.mousePosition - mouseLastPosition;

                // Get rotation and multiply by sensitivity
                rotationChange.y = mouseDrag.x * mouseSensitivity * Time.deltaTime;
                rotationChange.x = -(mouseDrag.y) * mouseSensitivity * Time.deltaTime;
            }
            // Otherwise, check numpad rotations
            else
            {
                // Check rotation inputs
                if (_inputManager.IsActionPressed<FlyCamLookLeftAction>())
                {
                    rotationChange += Vector2.down;
                }
                
                if (_inputManager.IsActionPressed<FlyCamLookUpAction>())
                {
                    rotationChange += Vector2.left;
                }
                
                if (_inputManager.IsActionPressed<FlyCamLookRightAction>())
                {
                    rotationChange += Vector2.up;
                }
                
                if (_inputManager.IsActionPressed<FlyCamLookDownAction>())
                {
                    rotationChange += Vector2.right;
                }
            }

            // Apply rotation
            Vector3 newRotation = transform.localEulerAngles + (Vector3)rotationChange;

            if (newRotation.x > 180) newRotation.x -= 360;

            newRotation.x = Mathf.Clamp(newRotation.x, -80, 80);
            transform.rotation = Quaternion.Euler(newRotation);

            // Accelerate the camera
            speed += acceleration;
            speed = Mathf.Min(speed, maxSpeed);

            // Move the FlyCam, stop the speed if the user is not moving
            if (isMoving)
            {
                if (_inputManager.IsActionPressed<FlyCamSpeedUpAction>())
                {
                    speed = speed * fastMultiplier;
                    transform.Translate(movementVector * speed * Time.deltaTime);
                }
                else if (_inputManager.IsActionPressed<FlyCamSlowDownAction>())
                {
                    speed = Mathf.Min(speed, maxSlowSpeed);
                    transform.Translate(movementVector * speed * Time.deltaTime);
                }
                else
                {
                    transform.Translate(movementVector * speed * Time.deltaTime);
                }
            }
            else
            {
                speed = 0;
            }

            // Update the mouse last position
            mouseLastPosition = Input.mousePosition;
        }
    }
}
