using System;
using UnityEngine;
using UnityEngine.UI;

/**
 * Handles the updates to the simulation status UI
 */
public class SimulationStatusPanelController : MonoBehaviour {

    private bool isVisible;

    private string timeAttempt = "NEVER";
    private string timeSuccess = "NEVER";

    private Image simulationStatusPanel;

    private Text simulationStatusTitle;

    private Text networkStatusText;
    private string networkStatusTitle = "// Network Status:";

    private Text coordinatesText;
    private string coordinatesTitle = "// Coordinates:";

    private InputManager inputManager;

    private void Awake()
    {
        simulationStatusPanel = GetComponent<Image>();
        simulationStatusTitle = GetComponentsInChildren<Text>()[0];
        networkStatusText = GetComponentsInChildren<Text>()[1];
        coordinatesText = GetComponentsInChildren<Text>()[2];

        // Subscribe to Inputs
        inputManager = FindObjectOfType<InputManager>();
        inputManager.onInfoPanelToggle += ToggleDisplay;
    }

    private void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        inputManager.onInfoPanelToggle += ToggleDisplay;
    }

    public void ToggleDisplay()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }

    public void Hide()
    {
        isVisible = false;
        simulationStatusPanel.enabled = false;
        simulationStatusTitle.enabled = false;
        networkStatusText.enabled = false;
        coordinatesText.enabled = false;
    }

    public void Show()
    {
        isVisible = true;
        simulationStatusPanel.enabled = true;
        simulationStatusTitle.enabled = true;
        networkStatusText.enabled = true;
        coordinatesText.enabled = true;
    }

    public void UpdateNetworkStatus(bool isSuccess, string result)
    {
        networkStatusText.text = networkStatusTitle;

        if (isSuccess)
            timeSuccess = DateTime.Now.ToLongTimeString();
        timeAttempt = DateTime.Now.ToLongTimeString();

        networkStatusText.text  +=  "\nLast Upload Time Attempt: " + timeAttempt;
        networkStatusText.text  +=  "\nLast Upload Time Success: " + timeSuccess;
        networkStatusText.text  +=  "\nLast Upload Result:       " + result;
    }

    public void UpdateCoordinates(string newText, string newTitle = "Coordinates")
    {
        coordinatesTitle = "// " + newTitle + ":";
        coordinatesText.text = coordinatesTitle;

        coordinatesText.text += "\n" + newText;
    }
}
