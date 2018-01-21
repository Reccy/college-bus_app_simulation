using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Visualises the bus route as a line renderer on the map
    /// </summary>
    [RequireComponent(typeof(BusDriver))]
    public class BusRouteVisualiser : MonoBehaviour
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
        /// Sets the new bus route to be displayed on the map using the attached LineRenderer component
        /// </summary>
        /// <param name="newRoute">The new route to display</param>
        public void SetBusRouteVisualisation(BusRoute newRoute)
        {
            List<BusRouteNode> nodes = newRoute.LatLongNodes;

            // Clear the old nodes
            foreach (GameObject oldNode in instantiatedNodes)
            {
                Destroy(oldNode.gameObject);
            }
            instantiatedNodes.Clear();

            // Create the new nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 nodePosition = nodes[i].AsUnityPosition(map);
                instantiatedNodes.Add(CreateBusRouteNodeGameObject("TestRoute", i, nodePosition));
            }

            // Generate mesh
            visualisation.positionCount = instantiatedNodes.Count;
            for (int i = 0; i < instantiatedNodes.Count; i++)
            {
                visualisation.SetPosition(i, instantiatedNodes[i].transform.position);
            }
        }

        /// <summary>
        /// Creates a new bus route node
        /// </summary>
        /// <param name="routeName">The name of the bus route</param>
        /// <param name="nodeIndex">The node index as part of the bus route</param>
        /// <param name="position">The position to place the bus route node</param>
        /// <returns>The bus route node</returns>
        private GameObject CreateBusRouteNodeGameObject(string routeName, int nodeIndex, Vector3 position)
        {
            // Create the node
            GameObject busRouteNode = new GameObject();
            busRouteNode.name = routeName + "_" + nodeIndex;

            // Set the node position
            busRouteNode.transform.position = position;

            // Snap the node to the terrain
            SnapToTerrain snapToTerrain = busRouteNode.AddComponent<SnapToTerrain>();
            snapToTerrain.SnapOnUpdate = false;
            snapToTerrain.PerformSnap();

            // Return the node
            return busRouteNode;
        }
    }
}
