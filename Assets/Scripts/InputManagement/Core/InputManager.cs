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
        /// Returns true if the bound key to the action is pressed down.
        /// </summary>
        /// <typeparam name="T">The InputAction to check.</typeparam>
        /// <returns>Returns true if the bound key to the action is pressed down.</returns>
        public bool IsActionDown<T>()
        {
            // Find any keybindings belonging to the action
            foreach (KeyBinding keyBinding in KeyBindings)
            {
                // Check if the bound key is pressed if the bound action matches generic T
                if (typeof(T).Equals(keyBinding.BoundAction.GetType()))
                {
                    // Return true if a key binding is pressed down
                    if (Input.GetKeyDown(keyBinding.BoundKey))
                        return true;
                    // Don't return false if not pressed as there may be multiple keys bound to an action
                }
            }

            // Return false if no keys were pressed
            return false;
        }

        /// <summary>
        /// Returns true if the bound key to the action is being held.
        /// </summary>
        /// <typeparam name="T">The InputAction to check.</typeparam>
        /// <returns>Returns true if the bound key to the action is being held.</returns>
        public bool IsActionPressed<T>()
        {
            // Find any keybindings belonging to the action
            foreach (KeyBinding keyBinding in KeyBindings)
            {
                // Check if the bound key is pressed if the bound action matches generic T
                if (typeof(T).Equals(keyBinding.BoundAction.GetType()))
                {
                    // Return true if a key binding is pressed
                    if (Input.GetKey(keyBinding.BoundKey))
                        return true;
                    // Don't return false if not pressed as there may be multiple keys bound to an action
                }
            }

            // Return false if no keys were pressed
            return false;
        }

        /// <summary>
        /// Returns true if the bound key to the action is released on this frame.
        /// </summary>
        /// <typeparam name="T">The InputAction to check.</typeparam>
        /// <returns>Returns true if the bound key to the action is released on this frame.</returns>
        public bool IsActionUp<T>()
        {
            // Find any keybindings belonging to the action
            foreach (KeyBinding keyBinding in KeyBindings)
            {
                // Check if the bound key is released if the bound action matches generic T
                if (typeof(T).Equals(keyBinding.BoundAction.GetType()))
                {
                    // Return true if a key binding is released
                    if (Input.GetKeyUp(keyBinding.BoundKey))
                        return true;
                    // Don't return false if not released as there may be multiple keys bound to an action
                }
            }

            // Return false if keys were not released
            return false;
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
                if (typeof(T).Equals(keyBinding.BoundAction.GetType()))
                {
                    return keyBinding.BoundAction;
                }
            }

            Debug.LogError("COULD NOT FIND ACTION! -> Action " + typeof(T) + " is not configured in the InputManager!");
            return null;
        }
    }
}
