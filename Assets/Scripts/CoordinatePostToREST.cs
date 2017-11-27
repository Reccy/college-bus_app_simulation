﻿using Mapbox.Unity.Location;
using Mapbox.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/**
 * Posts the coordinates to a REST API at a scheduled interval
 */
public class CoordinatePostToREST : MonoBehaviour {
    private TransformLocationProvider locationProvider;
    private SimulationStatusController statusController;

    // The interval in seconds to send the co-ordinates
    public int interval;

    // The URL to send the post to
    public string url;
    
    // Get the component on the game object
    void Awake()
    {
        locationProvider = GetComponent<TransformLocationProvider>();
        statusController = FindObjectOfType<SimulationStatusController>();
    }

    // Start the coroutine
    void Start()
    {
        StartCoroutine(PostAtInterval());
    }

    // Post the data at interval
    IEnumerator PostAtInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            Vector2d coords = locationProvider.Location;
            string latitude = coords.x.ToString();
            string longitude = coords.y.ToString();
            
            string coordsForm = "{\"latitude\":\"" + latitude + "\",\"longitude\":\"" + longitude + "\"}";

            UnityWebRequest request = UnityWebRequest.Post(url, coordsForm);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                statusController.UpdateNetworkStatus(false, request.error);
            }
            else
            {
                Debug.Log("Request Status:" + request.responseCode + " | Payload: " + coords.ToString());
                statusController.UpdateNetworkStatus(true, request.responseCode.ToString());
            }
        }
    }
}
