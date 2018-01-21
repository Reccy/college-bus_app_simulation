using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays the bus route of the attached bus driver script
/// </summary>
[RequireComponent (typeof (BusDriver))]
public class VisualiseBusRoute : MonoBehaviour
{
    [SerializeField]
    private AbstractMap map;

    public Material material;

    private List<GameObject> instantiatedNodes;
    private LineRenderer visualisation;

    private void Awake()
    {
        instantiatedNodes = new List<GameObject>();
        visualisation = gameObject.AddComponent<LineRenderer>();
        visualisation.material = material;
        visualisation.widthMultiplier = 0.2f;
        visualisation.positionCount = 0;
    }

    /// <summary>
    /// Displays the bus route on the map using the attached LineRenderer component
    /// </summary>
    /// <param name="newRoute">The new route to display</param>
    public void SetBusRouteVisualisation(BusRoute newRoute)
    {
        Debug.Log("Visualising bus route | COUNT: " + newRoute.Size);
        List<BusRouteNode> nodes = newRoute.LatLongNodes;
        
        // Clear the old nodes
        foreach (GameObject oldNode in instantiatedNodes)
        {
            Debug.Log("Destroying old node: " + oldNode.gameObject.name);
            Destroy(oldNode.gameObject);
        }
        instantiatedNodes.Clear();
        Debug.Log("Cleared instantiated nodes list");

        // Create the new nodes
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 nodePosition = nodes[i].AsUnityPosition(map);
            instantiatedNodes.Add(CreateBusRouteNodeGameObject("TestRoute", i, nodePosition));
            Debug.Log("Instantiated node: " + nodePosition);
        }

        // Generate mesh
        visualisation.positionCount = instantiatedNodes.Count;
        for (int i = 0; i < instantiatedNodes.Count; i++)
        {
            Debug.Log("Adding to mesh: " + instantiatedNodes[i].gameObject.name);
            visualisation.SetPosition(i, instantiatedNodes[i].transform.position);
        }
    }

    /// <summary>
    /// Creates a new bus route node to be placed in the scene
    /// </summary>
    /// <param name="routeName">The name of the bus route</param>
    /// <param name="nodeIndex">The node index as part of the bus route</param>
    /// <param name="position">The position to place the bus route node</param>
    /// <returns>The bus route node</returns>
    private GameObject CreateBusRouteNodeGameObject(string routeName, int nodeIndex, Vector3 position)
    {
        GameObject busRouteNode = new GameObject();
        busRouteNode.name = routeName + "_" + nodeIndex;
        busRouteNode.transform.position = position;
        SnapToTerrain snapToTerrain = busRouteNode.AddComponent<SnapToTerrain>();
        snapToTerrain.SnapOnUpdate = false;
        snapToTerrain.PerformSnap();
        return busRouteNode;
    }
}
