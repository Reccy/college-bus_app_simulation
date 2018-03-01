using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Responsible for keeping track of all bus stops in the simulation.
    /// Instantiates Bus Stops on Awake.
    /// </summary>
    public class BusStopManager : MonoBehaviour
    {
        private AbstractMap map;

        [NotNull]
        [SerializeField]
        private MapVisualizer mapVisualiser;

        [NotNull]
        [SerializeField]
        private GameObject busStopPrefab;

        [SerializeField]
        private List<BusStopData> busStopsData;
        /// <summary>
        /// List of all the Bus Stops in the simulation
        /// </summary>
        public List<BusStopData> BusStopsData
        {
            get { return busStopsData; }
        }

        private void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// Handles Bus Stop Manager logic when the active scene changes
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
                    }
                };
            }
        }

        /// <summary>
        /// Instantiates and places all of the Bus Stops into the map
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
        /// Ensures that each Bus Stop's data is valid
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
            // Check that each Bus Stop ID is unique
            ValidateBusStops();
        }
    }
}
