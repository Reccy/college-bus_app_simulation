using UnityEngine;
using Mapbox.Unity.Map;

// Copied from Mapbox example code:
// Mapbox\Examples\Scripts\LoadingPanelController.cs
public class UpdateUiOnMapLoaded : MonoBehaviour {
    public MapVisualizer MapVisualizer;
    public GameObject SetInvisible;
    public GameObject SetVisible;

    void Awake()
    {
        SetInvisible.SetActive(true);
        SetVisible.SetActive(false);

        MapVisualizer.OnMapVisualizerStateChanged += (s) =>
        {
            if (s == ModuleState.Finished)
            {
                SetInvisible.SetActive(false);
                SetVisible.SetActive(true);
            }
            else if (s == ModuleState.Working)
            {
                SetInvisible.SetActive(true);
                SetVisible.SetActive(false);
            }

        };
    }
}
