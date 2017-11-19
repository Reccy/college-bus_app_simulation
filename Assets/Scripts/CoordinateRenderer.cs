using Mapbox.Unity.Location;
using UnityEngine.UI;
using UnityEngine;

/**
 * Renders the coordinates to the UI
 */
public class CoordinateRenderer : MonoBehaviour {
    private TransformLocationProvider locationProvider;
    public Text textUI;

    // Get the component on the game object
    void Awake()
    {
        locationProvider = GetComponent<TransformLocationProvider>();
    }

    // Print location every frame
    void Update()
    {
        textUI.text = "Latitude: " + locationProvider.Location.x + "\nLongitude: " + locationProvider.Location.y;
    }

}
