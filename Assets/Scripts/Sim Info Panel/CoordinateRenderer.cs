using Mapbox.Unity.Location;
using UnityEngine.UI;
using UnityEngine;

/**
 * Renders the coordinates to the UI
 */
public class CoordinateRenderer : MonoBehaviour {
    private TransformLocationProvider locationProvider;
    private SimulationStatusPanelController statusController;
    public string prefix = "Coordinates";

    // Get the component on the game object
    void Awake()
    {
        locationProvider = GetComponent<TransformLocationProvider>();
        statusController = FindObjectOfType<SimulationStatusPanelController>();
    }

    // Print location every frame
    void Update()
    {
        statusController.UpdateCoordinates("Latitude: " + locationProvider.Location.x + "\nLongitude: " + locationProvider.Location.y);
    }

}
