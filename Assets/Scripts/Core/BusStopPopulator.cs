using AaronMeaney.BusStop.Core;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Debug class that allows the user to repopulate all bus stops
/// </summary>
public class BusStopPopulator : MonoBehaviour
{
    /// <summary>
    /// Repopulates all of the <see cref="BusStop"/>s in the scene
    /// </summary>
	public void PopulateBusStops()
    {
        foreach (BusStop stop in FindObjectsOfType<BusStop>())
        {
            stop.PopulateBusStop();
        }
    }
}

[CustomEditor(typeof(BusStopPopulator))]
public class BusRouteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            BusStopPopulator populator = (BusStopPopulator)target;

            if (GUILayout.Button("Repopulate Bus Stops"))
            {
                populator.PopulateBusStops();
            }
        }
    }
}
