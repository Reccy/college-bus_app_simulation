using UnityEngine;
using UnityEngine.SceneManagement;
using PubNubAPI;
using System.Collections.Generic;
using System;
using AaronMeaney.BusStop.Utilities;
using UnityEditor;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace AaronMeaney.BusStop.API
{
    /// <summary>
    /// Responsible for sending and receiving messages to the API.
    /// </summary>
    public class BusStopAPI : MonoBehaviour
    {
        /// <summary>
        /// Called when the API is initialized.
        /// </summary>
        public Action OnAPIInitialized;

        /// <summary>
        /// Called when the API receives a message.
        /// </summary>
        public Action<Dictionary<string, object>> OnMessageReceived;

        /// <summary>
        /// The <see cref="PubNub"/> instance.
        /// </summary>
        private PubNub pubnub;

        /// <summary>
        /// If the API is initialized.
        /// Set to true when the client can send a message to <see cref="PubNub"/>.
        /// </summary>
        private bool initialized = false;
        public bool Initialized
        {
            get { return initialized; }
        }

        /// <summary>
        /// The channel that the client is connecting to.
        /// </summary>
        public string PubNubChannel
        {
            get { return "bus_stop"; }
        }

        /// <summary>
        /// The base URL for the BusStopAPI.
        /// </summary>
        public string ApiBaseUrl
        {
            get { return "https://bus-stop-api.herokuapp.com/"; }
        }
        
        /// <summary>
        /// Initializes the PubNub API when the CityMap loads.
        /// </summary>
        private void Awake()
        {
            SceneManager.activeSceneChanged += (sceneFrom, sceneTo) =>
            {
                if (sceneTo.name.Equals("CityMap"))
                {
                    InitializePubNub();
                }
            };
        }
         
        /// <summary>
        /// Initializes the PubNub API.
        /// </summary>
        internal void InitializePubNub()
        {
            Debug.Log("[PubNub] Initializing PubNub.");

            // Setup and configure PubNub
            PNConfiguration pnConfiguration = new PNConfiguration();
            pnConfiguration.SubscribeKey = "sub-c-136ca9c4-4d58-11e8-9987-d26dac8959c0";
            pnConfiguration.PublishKey = "pub-c-c6e95d6d-cdba-4d5c-8c5d-29064d99fd0e";
            pnConfiguration.SecretKey = "sec-c-NWVkNmQwOWYtMDY5ZC00ZGZkLWI0NjYtMmNkZjMwMGEzZDIw";
            pnConfiguration.LogVerbosity = PNLogVerbosity.NONE;
            pnConfiguration.UUID = "UnityClient";

            pubnub = new PubNub(pnConfiguration);

            // Subscribe to Channel
            pubnub.SusbcribeCallback += (sender, e) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                if (mea.Status != null)
                {
                    if (mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                    {
                        Debug.Log("[PubNub] Subscribe Successful. Sending test message.");

                        PublishMessage("unity_connection_test", "This is just a connection test!", (result, status) => 
                        {
                            if (!status.Error)
                            {
                                Debug.Log("[PubNub] Successfully initialized PubNub!");
                                initialized = true;

                                try
                                {
                                    if (OnAPIInitialized != null)
                                        OnAPIInitialized();
                                } catch (Exception exception)
                                {
                                    Debug.LogException(exception);
                                }
                            }
                            else
                            {
                                Debug.Log(status.Error);
                                Debug.Log(status.ErrorData.Info);
                            }
                        });
                    }
                }
                if (mea.MessageResult != null)
                {
                    Debug.Log("[PubNub] Message Callback: " + mea.MessageResult.Payload);

                    try
                    {
                        if (mea.MessageResult.Payload != null)
                            HandleMessage((Dictionary<string, object>)mea.MessageResult.Payload);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
                if (mea.PresenceEventResult != null)
                {
                    Debug.Log("[PubNub] Presence Callback: " + mea.PresenceEventResult.Channel.ToString() + mea.PresenceEventResult.Occupancy.ToString() + mea.PresenceEventResult.Event.ToString());
                }
            };

            List<string> channelsList = new List<string>()
            {
                PubNubChannel
            };

            pubnub.Subscribe()
                .Channels(channelsList)
                .Execute();
        }
        
        /*
         * Message Dispatch
         */
        private void HandleMessage(Dictionary<string, object> payload)
        {
            try
            {
                string payloadString = "[PubNub Message Result] {\n";
                
                foreach (KeyValuePair<string, object> entry in payload)
                {
                    payloadString = payloadString + entry.Key + " : " + entry.Value + "\n";
                }

                payloadString = payloadString + "}";
                Debug.Log(payloadString);

                if (OnMessageReceived != null)
                    OnMessageReceived(payload);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        /*
         * Message Publishing
         */
        /// <summary>
        /// Sends a message to <see cref="PubNub"/>.
        /// Appends metadata to message.
        /// </summary>
        /// <param name="payload"><see cref="object"/> representing the message to send.</param>
        public void PublishMessage(string topic, object payload)
        {
            Dictionary<string, object> payloadDict = new Dictionary<string, object>();
            payloadDict.Add("message", payload);

            PublishMessage(topic, payloadDict);
        }

        /// <summary>
        /// Sends a message to <see cref="PubNub"/>.
        /// Appends metadata to message.
        /// Runs action when <see cref="PubNub"/> responds.
        /// </summary>
        /// <param name="payload"><see cref="object"/> representing the message to send.</param>
        /// <param name="callback"><see cref="Action{PNPublishResult, PNStatus}"/> to run when <see cref="PubNub"/> responds.</param>
        public void PublishMessage(string topic, object payload, Action<PNPublishResult, PNStatus> callback)
        {
            Dictionary<string, object> payloadDict = new Dictionary<string, object>();
            payloadDict.Add("message", payload);

            PublishMessage(topic, payloadDict, callback);
        }

        /// <summary>
        /// Sends a message to <see cref="PubNub"/>.
        /// Appends metadata to message.
        /// </summary>
        /// <param name="payload"><see cref="Dictionary{string, object}"/> representing JSON to be sent to <see cref="PubNub"/></param>
        public void PublishMessage(string topic, Dictionary<string, object> payload)
        {
            payload.Add("from", "unity_client");
            payload.Add("topic", topic);
            payload.Add("sent_at", DateTime.UtcNow.UnixTime().ToString());

            pubnub.Publish()
                .Channel(PubNubChannel)
                .Message(payload)
                .Async((result, status) => {
                    if (!status.Error)
                    {
                        Debug.Log("[PubNub] Successfully sent message to PubNub: " + payload.ToString());
                    }
                    else
                    {
                        Debug.Log(status.ErrorData.Info);
                    }
                });
        }

        /// <summary>
        /// Sends a message to <see cref="PubNub"/>.
        /// Appends metadata to message.
        /// </summary>
        /// <param name="payload"><see cref="Dictionary{string, object}"/> representing JSON to be sent to <see cref="PubNub"/></param>
        public void PublishMessage(string topic, Dictionary<string, object> payload, Action<PNPublishResult, PNStatus> callback)
        {
            payload.Add("from", "unity_client");
            payload.Add("topic", topic);
            payload.Add("sent_at", DateTime.UtcNow.UnixTime().ToString());

            pubnub.Publish()
                .Channel(PubNubChannel)
                .Message(payload)
                .Async((result, status) => callback(result, status));
        }

        /// <summary>
        /// Updates all of the <see cref="Core.BusStop"/>s stored on the API.
        /// </summary>
        /// <param name="busStops">The <see cref="List"/> of <see cref="Core.BusStop"/>s to be stored on the server.</param>
        public void UpdateBusStops(List<Core.BusStop> busStops)
        {
            JArray busStopsJson = new JArray();

            foreach (Core.BusStop busStop in busStops)
            {
                JObject busStopJson = new JObject();
                busStopJson.Add("id", busStop.BusStopId);
                busStopJson.Add("internal_id", busStop.BusStopIdInternal);
                busStopJson.Add("latitude", busStop.GetComponent<Core.PlaceAtCoordinates>().Latitude);
                busStopJson.Add("longitude", busStop.GetComponent<Core.PlaceAtCoordinates>().Longitude);

                busStopsJson.Add(busStopJson);
            }

            StartCoroutine(SendPostToServer("bus_stops", busStopsJson.ToString()));
        }

        /// <summary>
        /// Updates all of the <see cref="Core.BusRoute"/>s stored on the API.
        /// </summary>
        /// <param name="busRoutes">The <see cref="List"/> of <see cref="Core.BusRoute"/>s to be stores on the server.</param>
        public void UpdateBusRoutes(List<Core.BusRoute> busRoutes)
        {
            JArray busRoutesJson = new JArray();

            foreach (Core.BusRoute busRoute in busRoutes)
            {
                JObject busRouteJson = new JObject();
                busRouteJson.Add("id", busRoute.RouteId);
                busRouteJson.Add("internal_id", busRoute.RouteIdInternal);

                JArray busStops = new JArray();

                foreach (Core.RouteWaypoint routeWaypoint in busRoute.RouteWaypoints)
                {
                    if (routeWaypoint.LinkedBusStop)
                    {
                        busStops.Add(routeWaypoint.LinkedBusStop.BusStopIdInternal);
                    }
                }

                busRouteJson.Add("bus_stops", busStops);

                busRoutesJson.Add(busRouteJson);
            }

            Debug.Log("Routes being sent: " + busRoutesJson.Count);

            StartCoroutine(SendPostToServer("bus_routes", busRoutesJson.ToString()));
        }

        /// <summary>
        /// Sends a POST to the Bus Stop API.
        /// </summary>
        /// <param name="endpoint">The endpoint of the API method.</param>
        /// <param name="payload">The payload to send to the API.</param>
        /// /// <param name="callback">The code to run after the request completes.</param>
        /// <returns></returns>
        private IEnumerator SendPostToServer(string endpoint, string payload, Action<UnityWebRequest> callback = null)
        {
            string url = ApiBaseUrl + endpoint;
            Debug.Log("[Bus Stop API] Sending request to " + url + " with payload " + payload);

            UnityWebRequest request = UnityWebRequest.Post(url, payload);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("[Bus Stop API] Post error: " + request.error + ", => " + request.downloadHandler.text + ", => " + Encoding.UTF8.GetString(request.uploadHandler.data));
            }
            else
            {
                Debug.Log("[Bus Stop API] Post success:" + request.responseCode + "\nPayload: " + Encoding.UTF8.GetString(request.uploadHandler.data));
            }

            if (callback != null)
            {
                callback(request);
            }
        }
    }

    [CustomEditor(typeof(BusStopAPI))]
    public class BusStopAPIEditor : Editor
    {
        string topic = "Test topic.";
        string msg = "Test message from Unity.";
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            BusStopAPI api = (BusStopAPI)target;

            // Repaint the Editor when the api is ready
            api.OnAPIInitialized += () => Repaint();
            
            // Debug button to force PubNub to initialize
            if (Application.isPlaying)
            {
                if (!api.Initialized)
                {
                    if (GUILayout.Button("Force Init"))
                        api.InitializePubNub();
                }
                else
                {
                    EditorGUILayout.LabelField("Topic");
                    topic = EditorGUILayout.TextArea(topic);

                    EditorGUILayout.LabelField("Message");
                    msg = EditorGUILayout.TextArea(msg, GUILayout.MinHeight(75));

                    if (GUILayout.Button("Send Message"))
                        api.PublishMessage(topic, msg);
                }
            }
        }
    }
}
