using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Visualises the <see cref="BusPathfinder"/> as a <see cref="LineRenderer"/> on the map
    /// </summary>
    [RequireComponent(typeof(BusDriver))]
    public class BusPathfinderVisualiser : MonoBehaviour
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
        /// Sets the new <see cref="BusPathfinder"/> to be displayed on the map using the attached <see cref="LineRenderer"/> component
        /// </summary>
        /// <param name="newPath">The new <see cref="BusPathfinder"/> to visualise</param>
        public void SetBusPathVisualisation(BusPathfinder newPath)
        {
            List<CoordinateLocation> nodes = newPath.CoordinateLocations;

            // Clear the old nodes
            ClearNodes();

            // Create the new nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 nodePosition = nodes[i].AsUnityPosition(map);
                instantiatedNodes.Add(CreateBusPathNodeGameObject("TestRoute", i, nodePosition));
            }

            // Generate mesh
            visualisation.positionCount = instantiatedNodes.Count;
            for (int i = 0; i < instantiatedNodes.Count; i++)
            {
                visualisation.SetPosition(i, instantiatedNodes[i].transform.position);
            }
        }

        /// <summary>
        /// Clears the nodes in the visualisation
        /// </summary>
        public void ClearNodes()
        {
            // Check if instantiated nodes are null since this can be called before Awake()
            if (instantiatedNodes == null)
                return;

            foreach (GameObject oldNode in instantiatedNodes)
            {
                Destroy(oldNode.gameObject);
            }
            instantiatedNodes.Clear();
        }

        /// <summary>
        /// Clears the nodes in the visualisation and removes them from the <see cref="LineRenderer"/>.
        /// </summary>
        public void ClearVisualisation()
        {
            // Check if visualisation is null since this can be called before Awake()
            if (visualisation == null)
                return;

            ClearNodes();
            visualisation.positionCount = 0;
        }

        /// <summary>
        /// Creates a new <see cref="BusPathfinder"/> node
        /// </summary>
        /// <param name="pathName">The name of the <see cref="BusPathfinder"/> path</param>
        /// <param name="nodeIndex">The node index as part of the <see cref="BusPathfinder"/> path</param>
        /// <param name="position">The position to place the <see cref="BusPathfinder"/> path node</param>
        /// <returns>The <see cref="BusPathfinder"/> node</returns>
        private GameObject CreateBusPathNodeGameObject(string pathName, int nodeIndex, Vector3 position)
        {
            // Create the node
            GameObject busPathNode = new GameObject();
            busPathNode.name = pathName + "_" + nodeIndex;

            // Set the node position
            busPathNode.transform.position = position;

            // Snap the node to the terrain
            SnapToTerrain snapToTerrain = busPathNode.AddComponent<SnapToTerrain>();
            snapToTerrain.SnapOnUpdate = false;
            snapToTerrain.PerformSnap();

            // Return the node
            return busPathNode;
        }
    }
}
