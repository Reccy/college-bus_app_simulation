using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AaronMeaney.BusStop.Core;

namespace AaronMeaney.BusStop.API
{
    /// <summary>
    /// Pushes the Bus Route data to the REST API. Has to be called manually.
    /// </summary>
    public class BusRoutePostToREST : MonoBehaviour
    {
        [SerializeField]
        private bool debugLog;
        /// <summary>
        /// Whether or not to post debug log messages to the console.
        /// </summary>
        public bool DebugLog
        {
            get { return debugLog; }
            set { debugLog = value; }
        }

        [SerializeField]
        private string url;
        /// <summary>
        /// The URL to send the GET request to.
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        /// <summary>
        /// Sends the bus route data the the REST API.
        /// </summary>
        /// <param name="busRoute">The bus route to send</param>
        public void PostBusRoute(BusRoute busRoute)
        {
            StartCoroutine(Post(busRoute));
        }

        /// <summary>
        /// Sends the bus route data the the REST API. Asynchronous method.
        /// </summary>
        /// <param name="busRoute">The bus route to send</param>
        private IEnumerator Post(BusRoute busRoute)
        {
            List<BusRouteNode> nodeList = busRoute.LatLongNodes;
            string busRouteJSON = JsonConvert.SerializeObject(nodeList, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            if (DebugLog)
                Debug.Log("BusRoute Size: " + busRoute.Size + "\nJSON: " + busRouteJSON);

            UnityWebRequest request = UnityWebRequest.Post(url, busRouteJSON);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                if (debugLog)
                    Debug.Log(request.error);
            }
            else
            {
                if (debugLog)
                    Debug.Log("Request Status:" + request.responseCode + " | Payload: " + busRouteJSON);
            }
        }
    }
}
