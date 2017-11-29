using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;

/**
 *  Drives the bus to its waypoint
 */
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
    public AbstractMap Map { get { return map; } }


    [SerializeField]
    private float speed;

    /// <summary>
    /// The bus speed
    /// </summary>
    public float Speed { get { return speed; } set { speed = value; } }


    [SerializeField]
    private float busOriginDistanceFromRoad = 0.337f;

    /// <summary>
    /// The distance from the bus's origin to the road on the Y axis
    /// </summary>
    public float BusOriginDistanceFromRoad { get { return busOriginDistanceFromRoad; } set { busOriginDistanceFromRoad = value; } }


    [SerializeField]
    private BusDriverMode driverMode;
    
    /// <summary>
    /// The current driver mode of the bus
    /// </summary>
    public BusDriverMode DriverMode { get { return driverMode; } }


    [SerializeField]
    private Transform currentDestination;
    
    /// <summary>
    /// The transform that the bus is driving towards
    /// </summary>
    public Transform CurrentDestination { get { return currentDestination; } set { currentDestination = value; } }


    [SerializeField]
    private BusRoute busRoute;

    private int currentBusRouteNode = 0;

    private void Update()
    {
        // Rotate and Drive towards immediate destination if the bus is far away from it
        if (GetDistanceFromDestination() > 1)
        {
            RotateTowardsDestination();
            DriveForward();
        }
        else if (DriverMode.Equals(BusDriverMode.Route))
        {
            // Update the current destination
            if (currentBusRouteNode < busRoute.Size)
            {
                CurrentDestination.position = busRoute.LatLongNodes[currentBusRouteNode].AsUnityPosition(map);
                Debug.Log("Set bus route index: " + currentBusRouteNode + " || Position: " + CurrentDestination.position);
                currentBusRouteNode++;
                Debug.Log("Updated Index: " + currentBusRouteNode);
            }
        }

        SnapToTerrain();
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
        Transform originalTransform = transform;
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

    /// <summary>
    /// Makes the vehicle sit on the terrain by transforming its rotation and y transform
    /// </summary>
    private void SnapToTerrain()
    {
        // Get elevation
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hitPointList = Physics.RaycastAll(ray);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (hitPointList.Length > 0)
        {
            // Get the raycast hit point
            Vector3 hitPoint = hitPointList[0].point + new Vector3(0, busOriginDistanceFromRoad, 0);

            // Apply elevation
            transform.position = new Vector3(transform.position.x, hitPoint.y, transform.position.z);

            // Apply different angle
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hitPointList[0].normal));
        }
        else
        {
            // Move up since bus probably clipped under the terrain
            transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
        }
    }
}
