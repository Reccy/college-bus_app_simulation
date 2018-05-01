using UnityEngine;
using UnityEngine.SceneManagement;
using PubNubAPI;
using System.Collections.Generic;
using System;
using AaronMeaney.BusStop.Utilities;

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
        /// The <see cref="PubNub"/> instance.
        /// </summary>
        private PubNub pubnub;
        
        /// <summary>
        /// If the API is initialized.
        /// Set to true when the client can send a message to <see cref="PubNub"/>.
        /// </summary>
        public bool Initialized
        {
            get { return initialized; }
        }
        private bool initialized = false;

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
        private void InitializePubNub()
        {
            // Setup and configure PubNub
            PNConfiguration pnConfiguration = new PNConfiguration();
            pnConfiguration.SubscribeKey = "sub-c-136ca9c4-4d58-11e8-9987-d26dac8959c0";
            pnConfiguration.PublishKey = "pub-c-c6e95d6d-cdba-4d5c-8c5d-29064d99fd0e";
            pnConfiguration.SecretKey = "sec-c-NWVkNmQwOWYtMDY5ZC00ZGZkLWI0NjYtMmNkZjMwMGEzZDIw";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
            pnConfiguration.UUID = "UnityClient";

            pubnub = new PubNub(pnConfiguration);

            // Subscribe to Channel
            pubnub.SusbcribeCallback += (sender, e) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                if (mea.Status != null)
                {
                    if (mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                    {
                        PublishMessage("unity connection test", (result, status) => 
                        {
                            if (!status.Error)
                            {
                                Debug.Log("[PubNub] Successfully initialized PubNub!");
                                initialized = true;

                                if (OnAPIInitialized != null)
                                    OnAPIInitialized();
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
                    Debug.Log("[PubNub] Message Result: " + mea.MessageResult.Channel + mea.MessageResult.Payload);
                }
                if (mea.PresenceEventResult != null)
                {
                    Debug.Log("[PubNub[ Presence Callback: " + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event);
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

        /// <summary>
        /// Sends a message to <see cref="PubNub"/>.
        /// Appends metadata to message.
        /// </summary>
        /// <param name="payload"><see cref="string"/> representing the message to send.</param>
        public void PublishMessage(string payload)
        {
            Dictionary<string, string> payloadDict = new Dictionary<string, string>();
            payloadDict.Add("message", payload);

            PublishMessage(payloadDict);
        }

        /// <summary>
        /// Sends a message to <see cref="PubNub"/>.
        /// Appends metadata to message.
        /// Runs action when <see cref="PubNub"/> responds.
        /// </summary>
        /// <param name="payload"><see cref="string"/> representing the message to send.</param>
        /// <param name="callback"><see cref="Action{PNPublishResult, PNStatus}"/> to run when <see cref="PubNub"/> responds.</param>
        public void PublishMessage(string payload, Action<PNPublishResult, PNStatus> callback)
        {
            Dictionary<string, string> payloadDict = new Dictionary<string, string>();
            payloadDict.Add("message", payload);

            PublishMessage(payloadDict, callback);
        }

        /// <summary>
        /// Sends a message to <see cref="PubNub"/>.
        /// Appends metadata to message.
        /// </summary>
        /// <param name="payload"><see cref="Dictionary{string, string}"/> representing JSON to be sent to <see cref="PubNub"/></param>
        public void PublishMessage(Dictionary<string, string> payload)
        {
            payload.Add("from", "unity_client");
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
        /// <param name="payload"><see cref="Dictionary{string, string}"/> representing JSON to be sent to <see cref="PubNub"/></param>
        public void PublishMessage(Dictionary<string, string> payload, Action<PNPublishResult, PNStatus> callback)
        {
            payload.Add("from", "unity_client");
            payload.Add("sent_at", DateTime.UtcNow.UnixTime().ToString());

            pubnub.Publish()
                .Channel(PubNubChannel)
                .Message(payload)
                .Async((result, status) => callback(result, status));
        }
    }
}
