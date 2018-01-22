using UnityEngine;

namespace AaronMeaney.BusStop.Managers
{
    /// <summary>
    /// Responsible for managing the user's input by allowing other components to subscribe to the input events.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        // Control to toggle the Simulation Info Panel visibility
        public KeyCode ToggleInfoPanelKey;
        public delegate void ToggleInfoPanelDelegate();
        public ToggleInfoPanelDelegate onToggleInfoPanel;

        private void Update()
        {
            if (Input.GetKeyDown(ToggleInfoPanelKey))
            {
                if (onToggleInfoPanel != null)
                    onToggleInfoPanel();
            }
        }
    }
}
