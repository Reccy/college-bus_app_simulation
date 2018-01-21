using Mapbox.Unity.Location;
using UnityEngine;

/**
 * Prints the co-ordinates of the game object
 */
public class CoordinatesPrinter : MonoBehaviour {
    public TransformLocationProvider locationProvider;

    // Get the component on the game object
    void Awake ()
    {
        locationProvider = GetComponent<TransformLocationProvider>();
    }

    // Print location every frame
	void Update () {
        Debug.Log(gameObject.name + " coordinates: " + locationProvider.Location);
	}

}
