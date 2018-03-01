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
            transform.position = busStopData.AsUnityPosition(simAbstractMap);

            UpdatePosition();

            Debug.Log("Initialized -> " + busStopData.Identifier);
        }

        /// <summary>
        /// Updates the position based on the latitude and longitude of the Bus Stop Data
        /// </summary>
        public void UpdatePosition()
        {
            Debug.Log("Updating position of " + name);
            transform.position = busStopData.AsUnityPosition(simAbstractMap);

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
        private string identifier = "DEFAULT100";
        /// <summary>
        /// The unique identifier of the Bus Stop.
        /// </summary>
        public string Identifier
        {
            get { return identifier; }
        }

        [SerializeField]
        private float latitude;
        public float Latitude { get { return latitude; } }

        [SerializeField]
        private float longitude;
        public float Longitude { get { return longitude; } }

        /// <summary>
        /// The Bus Stop location as a Unity world position.
        /// </summary>
        /// <param name="map">The map to reference the latitude/longitude against to translate it to Unity world coordinated.</param>
        /// <returns>Position of the node in the world.</returns>
        public Vector3 AsUnityPosition(AbstractMap map)
        {
            Vector3 unityPosition = new Vector2(latitude, longitude).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);
            Debug.Log("LAT: " + latitude + ", LON: " + longitude + ", POS: " + unityPosition.ToString());
            return unityPosition;
        }
    }
}
