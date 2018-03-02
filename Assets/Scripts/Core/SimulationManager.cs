using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Responsible for keeping track of the simulation.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        private AbstractMap map;

        [NotNull]
        [SerializeField]
        private MapVisualizer mapVisualiser;

        [NotNull]
        [SerializeField]
        private GameObject busStopPrefab;

        [NotNull]
        [SerializeField]
        private GameObject routeWaypointPrefab;

        [SerializeField]
        private List<BusStopData> busStopsData;
        /// <summary>
        /// List of all the <see cref="BusStopData"/> to be added to the simulation
        /// </summary>
        public List<BusStopData> BusStopsData
        {
            get { return busStopsData; }
        }
        
        [SerializeField]
        private List<RouteWaypointData> routeWaypointsData;
        /// <summary>
        /// List of all the <see cref="RouteWaypointData"/> to be addeed to the simulation
        /// </summary>
        public List<RouteWaypointData> RouteWayPointsData
        {
            get { return routeWaypointsData; }
        }

        private void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// Handles <see cref="SimulationManager"/> logic when the active scene changes
        /// </summary>
        /// <param name="previousScene">The scene that was unloaded</param>
        /// <param name="currentScene">The scene that was just loaded</param>
        private void OnActiveSceneChanged(Scene previousScene, Scene currentScene)
        {
            Debug.Log("Changed Scene: " + currentScene.name);

            if (currentScene.Equals(SceneManager.GetSceneByName("CityMap")))
            {
                map = GameObject.FindGameObjectWithTag("Map").GetComponent<AbstractMap>();

                // Instantiate Bus Stops when the map is initialized
                mapVisualiser.OnMapVisualizerStateChanged += (s) =>
                {
                    if (s == ModuleState.Finished)
                    {
                        InstantiateBusStops();
                        InstantiateRouteWaypoints();
                    }
                };
            }
        }

        /// <summary>
        /// Instantiates and places all of the <see cref="BusStop"/>s into the map
        /// </summary>
        private void InstantiateBusStops()
        {
            foreach (BusStopData data in busStopsData)
            {
                GameObject newBusStop = Instantiate<GameObject>(busStopPrefab, Vector3.zero, Quaternion.identity);
                newBusStop.GetComponent<BusStop>().Initialize(data, map);
            }
        }

        /// <summary>
        /// Instantiates and places all of the <see cref="RouteWaypoint"/>s into the map
        /// </summary>
        private void InstantiateRouteWaypoints()
        {
            foreach (RouteWaypointData data in routeWaypointsData)
            {
                GameObject newRouteWaypoint = Instantiate<GameObject>(routeWaypointPrefab, Vector3.zero, Quaternion.identity);
                newRouteWaypoint.GetComponent<RouteWaypoint>().Initialize(data, map);
            }
        }

        /// <summary>
        /// Ensures that each <see cref="BusStop"/>'s data is valid
        /// </summary>
        private void ValidateBusStops()
        {
            List<string> ids = new List<string>();
            foreach (BusStopData busStopData in busStopsData)
            {
                if (ids.Contains(busStopData.Identifier))
                {
                    Debug.LogError("There are multiple Bus Stops with the ID: " + busStopData.Identifier);
                }
                else
                {
                    ids.Add(busStopData.Identifier);
                }
            }
        }

        private void OnValidate()
        {
            ValidateBusStops();
        }
    }
}
