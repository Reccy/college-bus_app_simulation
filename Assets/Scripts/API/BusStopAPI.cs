using UnityEngine;
using UnityEngine.SceneManagement;
using PubNubAPI;
using System.Collections.Generic;
using System;
using AaronMeaney.BusStop.Utilities;
using UnityEditor;

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
        /// Initializes the PubNub API when the .
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
                        Debug.Log(status.Error);
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
