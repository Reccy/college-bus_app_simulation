using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a waypoint along a <see cref="BusPathfinder"/>
    /// </summary>
    public class RouteWaypoint : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="BusStop"/> belonging to this <see cref="RouteWaypoint"/>
        /// </summary>
        public BusStop LinkedBusStop
        { get { return GetComponentInChildren<BusStop>(); } }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            if (LinkedBusStop)
            {
                Gizmos.DrawLine(transform.position, LinkedBusStop.transform.position);
            }
        }
    }
}
