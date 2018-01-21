using UnityEngine;

// Global input controller
public class InputManager : MonoBehaviour {

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
