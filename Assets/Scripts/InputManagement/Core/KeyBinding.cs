using UnityEngine;

namespace AaronMeaney.InputManagement
{
    /// <summary>
    /// Mapping between a KeyCode and an InputAction.
    /// InputManager responsible for calling the delegate in the bound action.
    /// </summary>
    [System.Serializable]
    public class KeyBinding
    {
        /// <summary>
        /// Name of the KeyBinding.
        /// No functional use, used for organising entries in the Inspector.
        /// </summary>
        public string name;

        /// <summary>
        /// The type of input action that the BoundKey should perform.
        /// </summary>
        public enum KeyPressType {
            /// <summary>
            /// The key is pressed down initially. Lasts for one frame.
            /// </summary>
            KEY_DOWN,
            /// <summary>
            /// The key is being held down. Can last for many frames.
            /// </summary>
            KEY_HOLD,
            /// <summary>
            /// The key is unpressed. Lasts for one frame.
            /// </summary>
            KEY_UP
        };

        [SerializeField]
        private KeyCode boundKey;
        /// <summary>
        /// The KeyCode used to perform the bound action.
        /// </summary>
        public KeyCode BoundKey
        {
            get { return boundKey; }
        }

        [SerializeField]
        private KeyPressType keyPress;
        /// <summary>
        /// The type of input action that the BoundKey should perform.
        /// </summary>
        public KeyPressType KeyPress
        {
            get { return keyPress; }
        }

        [SerializeField]
        private InputAction boundAction;
        /// <summary>
        /// The action that the bound key will perform.
        /// </summary>
        public InputAction BoundAction
        {
            get { return boundAction; }
        }
    }
}
