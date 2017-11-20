using UnityEngine;

/**
 *  Drives the bus to its waypoint
 */
public class BusDriver : MonoBehaviour {

    public float speed;
    private float busDistanceFromRoad = 0.337f;
    
    void Update()
    {
        DriveForward();
    }

    /**
     *  Makes the vehicle drive forward, connected to the terrain
     */
    void DriveForward()
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
            Vector3 hitPoint = hitPointList[0].point + new Vector3(0, busDistanceFromRoad, 0);

            // Apply elevation
            transform.position = new Vector3(transform.position.x, hitPoint.y, transform.position.z);

            // Apply different angle
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hitPointList[0].normal));

            // Move forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            // Move up since bus probably clipped under the terrain
            transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
        }
    }
}
