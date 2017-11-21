using Mapbox.Unity.Location;
using Mapbox.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/**
 * Posts the coordinates to a REST API at a scheduled interval
 */
public class CoordinatePostToREST : MonoBehaviour {
    public TransformLocationProvider locationProvider;

    // The interval in seconds to send the co-ordinates
    public int interval;

    // The URL to send the post to
    public string url;

    // Get the component on the game object
    void Awake()
    {
        locationProvider = GetComponent<TransformLocationProvider>();
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
            }
            else
            {
                Debug.Log("Request Status:" + request.responseCode + " | Payload: " + coords.ToString());
            }
        }
    }
}
