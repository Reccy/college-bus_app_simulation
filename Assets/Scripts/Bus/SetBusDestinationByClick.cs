using UnityEngine;
using UnityEngine.EventSystems;

/**
 *  Sets the position of this game object by click
 */
public class SetBusDestinationByClick : MonoBehaviour
{
    [SerializeField]
    private BusDriver busDriver;
    public BusDriver BusDriver { get { return busDriver; } set { busDriver = value; } }

    private void Update()
    {
        switch (BusDriver.DriverMode)
        {
            case BusDriver.BusDriverMode.Debug:
                SetCurrentDestinationOnClick();
                break;
            case BusDriver.BusDriverMode.Route:
                SetRouteDestinationOnClick();
                break;
        }
    }

    private void SetCurrentDestinationOnClick()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (hits.Length > 0)
            {
                transform.position = hits[0].point;
            }
        }
    }

    private void SetRouteDestinationOnClick()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (hits.Length > 0)
            {
                // Assign the new bus route
                BusRoute newBusRoute = new BusRoute();
                newBusRoute.SetDirectionsToPosition(BusDriver.Map, BusDriver.transform.position, hits[0].point);
                BusDriver.CurrentBusRoute = newBusRoute;
            }
        }
    }
}
