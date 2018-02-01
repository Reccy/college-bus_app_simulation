using UnityEngine;

namespace AaronMeaney.BusStop.API
{
    /// <summary>
    /// Configuration data used for the BusStop API
    /// </summary>
    [CreateAssetMenu(fileName = "BusStopAPIConfiguration_Empty.asset", menuName = "Bus Stop/API Config")]
    public class BusStopAPIConfiguration : ScriptableObject
    {
        /// <summary>
        /// Base URL of the API.
        /// For example: www.busapp.com/api/
        /// </summary>
        public string baseUrl;
    }
}
