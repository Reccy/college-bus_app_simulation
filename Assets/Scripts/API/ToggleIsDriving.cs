using System.Collections;
using AaronMeaney.BusStop.Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AaronMeaney.BusStop.API.Action
{
    /// <summary>
    /// Polls the REST API to check if the bus should be driving or not.
    /// </summary>
    [RequireComponent(typeof(BusDriver))]
    public class ToggleIsDriving : MonoBehaviour
    {
        /// <summary>
        /// Reference to the bus driver component.
        /// </summary>
        private BusDriver busDriver;

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
        /// The interval in seconds to send get the bus driving state.
        /// </summary>
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
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

        private void Awake()
        {
            busDriver = GetComponent<BusDriver>();
        }

        private void Start()
        {
            StartCoroutine(Poll());
        }

        /// <summary>
        /// Polls the API every "interval" seconds.
        /// </summary>
        private IEnumerator Poll()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);

                UnityWebRequest request = UnityWebRequest.Get(url);
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    if (debugLog)
                        Debug.Log(request.error);
                }
                else
                {
                    while (!request.downloadHandler.isDone)
                        yield return new WaitForEndOfFrame();

                    BusIsDrivingResponse response = JsonConvert.DeserializeObject<BusIsDrivingResponse>(request.downloadHandler.text);

                    if (debugLog)
                    {
                        Debug.Log("================");
                        Debug.Log(request.downloadHandler.text);
                        Debug.Log("Request Status:" + request.responseCode + " | Result: " + response.IsDriving);
                    }

                    busDriver.IsDriving = response.IsDriving;
                }
            }
        }
    }

    /// <summary>
    /// Wrapper for response from the API.
    /// </summary>
    [System.Serializable]
    public class BusIsDrivingResponse
    {
        [SerializeField]
        [JsonProperty("success")]
        private bool success;
        /// <summary>
        /// If the response was successful.
        /// </summary>
        public bool Success
        {
            get { return success; }
        }

        [SerializeField]
        [JsonProperty("is_driving")]
        private bool isDriving;
        /// <summary>
        /// If the bus is driving or not.
        /// </summary>
        public bool IsDriving
        {
            get { return isDriving; }
        }

        public BusIsDrivingResponse(bool success, bool isDriving)
        {
            this.success = success;
            this.isDriving = isDriving;
        }
    }
}
