using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents the latitude, longitude and Unity world position of a bus route node.
    /// </summary>
    [System.Serializable]
    public class BusRouteNode
    {
        [SerializeField]
        private float latitude;
        public float Latitude { get { return latitude; } }

        [SerializeField]
        private float longitude;
        public float Longitude { get { return longitude; } }

        public BusRouteNode(float latitude, float longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public BusRouteNode(Vector2 latLong)
        {
            this.latitude = latLong.x;
            this.longitude = latLong.y;
        }

        /// <summary>
        /// The Bus Route Node as a Unity world position.
        /// </summary>
        /// <param name="map">The map to reference the latitude/longitude against to translate it to Unity world coordinated.</param>
        /// <returns>Position of the node in the world.</returns>
        public Vector3 AsUnityPosition(AbstractMap map)
        {
            Vector3 unityPosition = new Vector2(latitude, longitude).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);
            return unityPosition;
        }
    }
}
