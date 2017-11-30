using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Pushes the BusRoute to the REST API. Has to be called manually
/// </summary>
public class BusRoutePostToREST : MonoBehaviour
{
    public bool debugLog;
    public string url;

    /// <summary>
    /// Sends the bus route data the the REST API
    /// </summary>
    /// <param name="busRoute">The bus route to send</param>
    public void PostBusRoute(BusRoute busRoute)
    {
        StartCoroutine(Post(busRoute));
    }

    private IEnumerator Post(BusRoute busRoute)
    {
        List<BusRouteNode> nodeList = busRoute.LatLongNodes;
        string busRouteJSON = JsonConvert.SerializeObject(nodeList, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        Debug.Log("BusRoute Size: " + busRoute.Size + "\nJSON: " + busRouteJSON);

        UnityWebRequest request = UnityWebRequest.Post(url, busRouteJSON);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.Send();

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
