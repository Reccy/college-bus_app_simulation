using UnityEngine;

namespace AaronMeaney.InputManagement
{
    /// <summary>
    /// Represents the action to be taken by the simulation.
    /// InputManager calls the onActionPerformed() delegate when the relevant KeyBinding is triggered.
    /// </summary>
    [System.Serializable]
    public abstract class InputAction : MonoBehaviour
    {
        public delegate void OnActionPerformed();
        /// <summary>
        /// The delegate that is called when the action is performed.
        /// Should only be called from the InputManager class.
        /// Subscribe to this from other classes.
        /// </summary>
        public OnActionPerformed onActionPerformed;

        /// <summary>
        /// Method used to perform the action of this class.
        /// </summary>
        public void performAction()
        {
            if (onActionPerformed != null)
                onActionPerformed();
        }
    }
}
