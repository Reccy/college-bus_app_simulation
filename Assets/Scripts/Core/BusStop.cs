using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a <see cref="BusStop"/> on the side of the road
    /// </summary>
    public class BusStop : MonoBehaviour
    {
        [SerializeField]
        private string busStopId;
        /// <summary>
        /// The ID of the <see cref="BusStop"/>. Must be unique.
        /// </summary>
        public string BusStopId { get { return busStopId; } }
        
        /// <summary>
        /// The <see cref="RouteWaypoint"/> belonging to this <see cref="BusStop"/>
        /// </summary>
        public RouteWaypoint LinkedRouteWaypoint
        {
            get { return GetComponentInParent<RouteWaypoint>(); }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            if (LinkedRouteWaypoint)
            {
                Gizmos.DrawLine(transform.position, LinkedRouteWaypoint.transform.position);
            }
        }
    }
}
