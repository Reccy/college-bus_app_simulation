using AaronMeaney.BusStop.UI.SimulationStatusPanel;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AaronMeaney.BusStop.API.Action
{
    /// <summary>
    /// Posts the coordinates to a REST API at a scheduled interval
    /// </summary>
    public class PostCoordinates : MonoBehaviour
    {
        private TransformLocationProvider locationProvider;
        private SimulationStatusPanelController statusController;

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
        private int interval;
        /// <summary>
        /// The interval in seconds to to wait between sending the co-ordinates.
        /// </summary>
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        [SerializeField]
        private string url;
        /// <summary>
        /// The URL to send the post to.
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        void Awake()
        {
            locationProvider = GetComponent<TransformLocationProvider>();
            statusController = FindObjectOfType<SimulationStatusPanelController>();
        }

        void Start()
        {
            StartCoroutine(PostAtInterval());
        }

        /// <summary>
        /// Post the data to the REST API every "interval" seconds.
        /// </summary>
        /// <returns></returns>
        IEnumerator PostAtInterval()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);

                Vector2d coords = locationProvider.Location;
                string latitude = coords.x.ToString();
                string longitude = coords.y.ToString();

                string coordsJSON = "{\"latitude\":\"" + latitude + "\",\"longitude\":\"" + longitude + "\"}";

                UnityWebRequest request = UnityWebRequest.Post(url, coordsJSON);
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    if (debugLog)
                        Debug.Log(request.error);

                    statusController.UpdateNetworkStatus(false, request.error);
                }
                else
                {
                    if (debugLog)
                        Debug.Log("Request Status:" + request.responseCode + " | Payload: " + coords.ToString());

                    statusController.UpdateNetworkStatus(true, request.responseCode.ToString());
                }
            }
        }
    }
}
