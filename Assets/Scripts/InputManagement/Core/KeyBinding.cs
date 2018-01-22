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
