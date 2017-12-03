using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;

/// <summary>
/// Responsible for the bus's pathfinding and other functionalities
/// </summary>
 [RequireComponent (typeof (TransformLocationProvider))]
public class BusDriver : MonoBehaviour {

    /// <summary>
    /// How the bus driver will get to its destination
    /// </summary>
    public enum BusDriverMode
    {
        /// <summary>
        /// The user clicks to make the bus drive to its destination
        /// </summary>
        Debug,
        /// <summary>
        /// The bus follows its bus route
        /// </summary>
        Route
    }

    [SerializeField]
    private AbstractMap map;
    /// <summary>
    /// The map that the bus is driving on
    /// </summary>
    public AbstractMap Map {
        get { return map; }
    }

    [SerializeField]
    private float speed;
    /// <summary>
    /// The bus speed
    /// </summary>
    public float Speed {
        get { return speed; }
        set { speed = value; }
    }

    private bool isDriving;
    /// <summary>
    /// If the bus is driving/stopped
    /// </summary>
    public bool IsDriving
    {
        get { return isDriving; }
        set { isDriving = value; }
    }
    
    [SerializeField]
    private float busOriginDistanceFromRoad = 0.337f;
    /// <summary>
    /// The distance from the bus's origin to the road on the Y axis
    /// </summary>
    public float BusOriginDistanceFromRoad {
        get { return busOriginDistanceFromRoad; }
        set { busOriginDistanceFromRoad = value; }
    }

    [SerializeField]
    private BusDriverMode driverMode;

    /// <summary>
    /// The current driver mode of the bus
    /// </summary>
    public BusDriverMode DriverMode {
        get { return driverMode; }
    }

    [SerializeField]
    private Transform currentDestination;
    /// <summary>
    /// The transform that the bus is driving towards
    /// </summary>
    public Transform CurrentDestination {
        get { return currentDestination; }
        set { currentDestination = value; }
    }

    private BusRoute currentBusRoute;
    /// <summary>
    /// The bus route currently assigned to the bus
    /// </summary>
    public BusRoute CurrentBusRoute {
        get { return currentBusRoute; }
        set
        {
            currentBusRouteNode = 0;
            currentBusRoute = value;

            // If a VisualiseBusRoute component exists, then visualise the route once the bus route has been populated
            if (GetComponent<VisualiseBusRoute>())
            {
                CurrentBusRoute.onBusRoutePopulated += GetComponent<VisualiseBusRoute>().SetBusRouteVisualisation;
            }

            // If a BusRoutePostToREST component exists, the send to the API once the bus route has been populated
            if (GetComponent<BusRoutePostToREST>())
            {
                CurrentBusRoute.onBusRoutePopulated += GetComponent<BusRoutePostToREST>().PostBusRoute;
            }
        }
    }

    private int currentBusRouteNode = 0;

    private void Awake()
    {
        CurrentBusRoute = new BusRoute();
    }

    private void Update()
    {
        // Rotate and Drive towards immediate destination if the bus is far away from it
        if (GetDistanceFromDestination() > 1)
        {
            if (isDriving)
            {
                RotateTowardsDestination();
                DriveForward();
            }
        }
        else if (DriverMode.Equals(BusDriverMode.Route))
        {
            // Update the current destination
            if (currentBusRouteNode < currentBusRoute.Size)
            {
                CurrentDestination.position = currentBusRoute.LatLongNodes[currentBusRouteNode].AsUnityPosition(map);
                currentBusRouteNode++;
            }
        }
    }

    /// <summary>
    /// Returns the distance from the current destination
    /// </summary>
    private float GetDistanceFromDestination()
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentDestination.position.x, currentDestination.position.z));
    }

    /// <summary>
    /// Makes the vehicle rotate towards the destination
    /// </summary>
    private void RotateTowardsDestination()
    {
        transform.LookAt(currentDestination);
    }

    /// <summary>
    /// Drives the vehicle forward by translating its transform position forward
    /// </summary>
    private void DriveForward()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
