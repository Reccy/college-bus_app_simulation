using UnityEngine;

/**
 *  Drives the bus to its waypoint
 */
public class BusDriver : MonoBehaviour {

    public float speed;
    public float busOriginDistanceFromRoad = 0.337f;
    public Transform busDestination;

    void Update()
    {
        // Rotate and Drive towards destination if far away from it
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(busDestination.position.x, busDestination.position.z)) > 1)
        {
            RotateTowardsDestination();
            DriveForward();
        }

        UpdateY();
    }

    /**
     *  Makes the vehicle rotate towards the destination
     */
    void RotateTowardsDestination()
    {
        Transform originalTransform = transform;
        transform.LookAt(busDestination);
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
    }

    /**
     *  Makes the vehicle sit on the terrain
     */
    void UpdateY()
    {
        // Get elevation
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hitPointList = Physics.RaycastAll(ray);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (hitPointList.Length > 0)
        {
            // Debug hit tag and xyz coordinates
            Debug.Log("HIT: " + hitPointList[0].collider.tag + " - XYZ: " + hitPointList[0].point);

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
    void DriveForward()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
