using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.InputManagement
{
    /// <summary>
    /// Responsible for managing the user's input by allowing other components to subscribe to the input events.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private List<KeyBinding> keyBindings;
        /// <summary>
        /// The list of key bindings used to configure the input.
        /// </summary>
        public List<KeyBinding> KeyBindings
        {
            get { return keyBindings; }
        }

        private void Update()
        {
            // List of executed actions this frame.
            List<InputAction> executedActions = new List<InputAction>();

            foreach (KeyBinding keyBinding in KeyBindings)
            {
                // If this action has already been executed on this frame, break to prevent calling the action multiple times
                if (executedActions.Contains(keyBinding.BoundAction))
                    break;

                // If the bound key is pressed, execute the bound action
                switch (keyBinding.KeyPress) {
                    case KeyBinding.KeyPressType.KEY_DOWN:
                        if (Input.GetKeyDown(keyBinding.BoundKey))
                        {
                            keyBinding.BoundAction.PerformAction();
                        }
                        break;

                    case KeyBinding.KeyPressType.KEY_HOLD:
                        if (Input.GetKey(keyBinding.BoundKey))
                        {
                            keyBinding.BoundAction.PerformAction();
                        }
                        break;

                    case KeyBinding.KeyPressType.KEY_UP:
                        if (Input.GetKeyUp(keyBinding.BoundKey))
                        {
                            keyBinding.BoundAction.PerformAction();
                        }
                        break;
                }
            }

            executedActions.Clear();
        }

        /// <summary>
        /// Returns the concrete instance of the InputAction.
        /// </summary>
        /// <typeparam name="T">The type of the concrete InputAction that's expected.</typeparam>
        /// <returns>The concrete InputAction if configured in a KeyBinding. Null if InputAction is not configured in a KeyBinding.</returns>
        public InputAction GetAction<T>()
        {
            foreach (KeyBinding keyBinding in KeyBindings)
            {
                Debug.Log("CHECKING " + keyBinding.BoundAction.GetType() + " AGAINST " + typeof(T));

                if (typeof(T).Equals(keyBinding.BoundAction.GetType()))
                {
                    Debug.Log("MATCH!");
                    return keyBinding.BoundAction;
                }
            }

            Debug.LogError("COULD NOT FIND ACTION! -> Action " + typeof(T) + " is not configured in the InputManager!");
            return null;
        }
    }
}
