using UnityEngine;

namespace AaronMeaney.BusStop.Managers
{
    /// <summary>
    /// Responsible for managing the user's input by allowing other components to subscribe to delegates.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        // Control to toggle the Simulation Info Panel visibility
        public KeyCode ToggleInfoPanel;
        public delegate void ToggleInfoPanelDelegate();
        public ToggleInfoPanelDelegate onInfoPanelToggle;

        private void Update()
        {
            if (Input.GetKeyDown(ToggleInfoPanel))
            {
                if (onInfoPanelToggle != null)
                    onInfoPanelToggle();
            }
        }
    }
}
