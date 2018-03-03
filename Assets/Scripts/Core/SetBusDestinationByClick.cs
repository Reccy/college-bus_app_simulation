using UnityEngine;
using UnityEngine.EventSystems;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Sets the bus's route destination when the user clicks on a location on the map
    /// </summary>
    public class SetBusDestinationByClick : MonoBehaviour
    {
        [SerializeField]
        private BusDriver busDriver;
        /// <summary>
        /// Determines the drive mode of the bus.
        /// </summary>
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

        /// <summary>
        /// Sets the immediate straight-line destination for the bus.
        /// </summary>
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

        /// <summary>
        /// Sets the route destination for the bus.
        /// </summary>
        private void SetRouteDestinationOnClick()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                if (hits.Length > 0)
                {
                    // Assign the new bus route
                    BusPathfinder newBusRoute = new BusPathfinder();
                    newBusRoute.SetDirectionsToPosition(BusDriver.Map, BusDriver.transform.position, hits[0].point);
                    BusDriver.CurrentBusRoute = newBusRoute;
                }
            }
        }
    }
}
