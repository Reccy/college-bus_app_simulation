using UnityEngine;

/// <summary>
/// Ensures that the object's Transform.position.y matches the terrain below or above it
/// </summary>
public class SnapToTerrain : MonoBehaviour
{
    [SerializeField]
    private float originDistanceFromRoad = 0.5f;

    [SerializeField]
    private bool alignAngleWithTerrain = false;

    [SerializeField]
    private float rayOriginHeight = 100f;

    [SerializeField]
    private bool snapOnUpdate = true;
    /// <summary>
    /// If set to true, PerformSnap() will be called in update function
    /// </summary>
    public bool SnapOnUpdate
    {
        get { return snapOnUpdate; }
        set { snapOnUpdate = value; }
    }

    private void Update()
    {
        if (snapOnUpdate)
            PerformSnap();   
    }

    /// <summary>
    /// Snaps the object to terrain by casting rays below it from a high altitude and by then snapping at the collision point
    /// </summary>
    public void PerformSnap()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x, rayOriginHeight, transform.position.z);
        Vector3 rayDirection = Vector3.down * rayOriginHeight * 2;
        Ray downRay = new Ray(rayOrigin, rayDirection);
        RaycastForTerrain(downRay);
    }

    /// <summary>
    /// Performs a raycast and sets the object y position if the raycast hits a collider
    /// </summary>
    /// <param name="ray">The ray to perform the raycast with</param>
    private void RaycastForTerrain(Ray ray)
    {
        RaycastHit[] hitPointList = Physics.RaycastAll(ray);
        Debug.DrawRay(ray.origin, ray.direction * rayOriginHeight * 2, Color.red);

        if (hitPointList.Length > 0)
        {
            // Get the raycast hit point
            Vector3 hitPoint = hitPointList[0].point + new Vector3(0, originDistanceFromRoad, 0);

            // Apply elevation
            transform.position = new Vector3(transform.position.x, hitPoint.y, transform.position.z);
            
            if (alignAngleWithTerrain)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hitPointList[0].normal));
            }
        }
    }
}
