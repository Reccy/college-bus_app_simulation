using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Handles Bus Stop functionality.
    /// </summary>
    [System.Serializable]
    [RequireComponent(typeof(SnapToTerrain))]
    public class BusStop : MonoBehaviour
    {
        private AbstractMap simAbstractMap;

        [SerializeField]
        private BusStopData busStopData;
        /// <summary>
        /// The Bus Stop's data
        /// </summary>
        public BusStopData BusStopData {
            get { return busStopData; }
        }

        /// <summary>
        /// Sets the Bus Stop's data and places it in the map
        /// </summary>
        public void Initialize(BusStopData busStopData, AbstractMap simAbstractMap)
        {
            this.busStopData = busStopData;
            this.simAbstractMap = simAbstractMap;

            // Set GameObject name and position
            name = "Bus Stop " + busStopData.Identifier;
            transform.position = busStopData.coordinateLocation.AsUnityPosition(simAbstractMap);

            UpdatePosition();

            Debug.Log("Initialized -> " + busStopData.Identifier);
        }

        /// <summary>
        /// Updates the position based on the latitude and longitude of the Bus Stop Data
        /// </summary>
        public void UpdatePosition()
        {
            Debug.Log("Updating position of " + name);
            transform.position = busStopData.coordinateLocation.AsUnityPosition(simAbstractMap);

            // Snap position to terrain
            Debug.Log("Snapping " + name + " to terrain");
            GetComponent<SnapToTerrain>().PerformSnap();
        }
    }

    /// <summary>
    /// Contains Bus Stop data.
    /// </summary>
    [System.Serializable]
    public class BusStopData
    {
        [SerializeField]
        private string identifier = "DEFAULT";
        /// <summary>
        /// The unique identifier of the Bus Stop.
        /// </summary>
        public string Identifier
        {
            get { return identifier; }
        }
        
        /// <summary>
        /// The coordinates of the bus stop
        /// </summary>
        [SerializeField]
        public CoordinateLocation coordinateLocation;
    }
}
