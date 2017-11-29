using UnityEngine;

/**
 *  Drives the bus to its waypoint
 */
public class BusDriver : MonoBehaviour {

    /**
     * How the bus driver will get to its destination.
     * Debug = The user clicks to make the bus drive to its destination
     * Route = The bus follows its route
     */
    public enum BusDriverMode
    {
        Debug,
        Route
    }

    [SerializeField]
    private float speed;
    public float Speed { get; set; }

    [SerializeField]
    private float busOriginDistanceFromRoad = 0.337f;
    public float BusOriginDistanceFromRoad { get; set; }

    [SerializeField]
    private BusDriverMode currentDriverMode;
    public BusDriverMode CurrentDriverMode { get { return currentDriverMode; } }

    [SerializeField]
    private Transform busImmediateDestination;
    public Transform BusImmediateDestination { get; set; }

    [SerializeField]
    private BusRoute busRoute;
    

    private void Update()
    {
        // Rotate and Drive towards immediate destination if the bus is far away from it
        if (GetBusDistanceFromImmediateDestination() > 1)
        {
            RotateTowardsDestination();
            DriveForward();
        }

        UpdateY();
    }

    /**
     * Returns the distance from the immediate destination
     */
    private float GetBusDistanceFromImmediateDestination()
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(busImmediateDestination.position.x, busImmediateDestination.position.z));
    }

    /**
     *  Makes the vehicle rotate towards the destination
     */
    private void RotateTowardsDestination()
    {
        Transform originalTransform = transform;
        transform.LookAt(busImmediateDestination);
    }

    /**
     *  Makes the vehicle sit on the terrain
     */
    private void UpdateY()
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

    /**
     * Drives the vehicle forward
     */
    private void DriveForward()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
