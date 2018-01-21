using UnityEngine;
using Mapbox.Unity.Map;

// Copied from Mapbox example code:
// Mapbox\Examples\Scripts\LoadingPanelController.cs
public class HideObjectOnMapLoaded : MonoBehaviour
{
    public MapVisualizer MapVisualizer;
    public GameObject Content;

    void Awake()
    {
        MapVisualizer.OnMapVisualizerStateChanged += (s) =>
        {
            if (s == ModuleState.Finished)
            {
                Content.SetActive(false);
            }
            else if (s == ModuleState.Working)
            {
                Content.SetActive(true);
            }

        };
    }
}
