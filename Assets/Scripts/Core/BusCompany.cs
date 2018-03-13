using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a <see cref="BusCompany"/>
    /// </summary>
    public class BusCompany : MonoBehaviour
    {
        [SerializeField]
        private string companyName;
        /// <summary>
        /// The name of the <see cref="BusCompany"/>
        /// </summary>
        public string CompanyName { get { return companyName; } }

        [SerializeField]
        private Color busPaintColor;
        /// <summary>
        /// The <see cref="Color"/> that the busses should be painted for this <see cref="BusCompany"/>
        /// </summary>
        public Color BusPaintColor { get { return busPaintColor; } }

        /// <summary>
        /// The list of <see cref="BusTimetable"/>s that belong to the <see cref="BusCompany"/>
        /// </summary>
        public List<BusTimetable> BusTimetables { get { return new List<BusTimetable>(GetComponentsInChildren<BusTimetable>()); } }

        /// <summary>
        /// The list of <see cref="BusRoute"/>s that belong of the <see cref="BusCompany"/>
        /// </summary>
        public List<BusRoute> BusRoutes { get { return new List<BusRoute>(GetComponentsInChildren<BusRoute>()); } }

        /// <summary>
        /// List of <see cref="Bus"/> <see cref="GameObject"/>s that are ready to be taken from the depot.
        /// </summary>
        private List<GameObject> bussesInDepot = new List<GameObject>();

        /// <summary>
        /// List of <see cref="Bus"/> <see cref="GameObject"/>s that are currently out of the depot.
        /// </summary>
        private List<GameObject> bussesOnRoad = new List<GameObject>();

        /// <summary>
        /// Deploys an instance of a <see cref="Bus"/> <see cref="GameObject"/> from the depot to a specific <see cref="BusService"/>
        /// </summary>
        /// <param name="service">The <see cref="BusService"/> to assign the bus to
        public void DeployBus(BusService service)
        {
            if (bussesInDepot.Count == 0)
            {
                Debug.Log("The " + CompanyName + " Company depot is empty.");
                return;
            }

            GameObject bus = bussesInDepot[0];
            bussesOnRoad.Add(bus);
            bussesInDepot.Remove(bus);

            bus.GetComponent<Bus>().StartService(service);
        }

        /// <summary>
        /// Returns an instance of a <see cref="Bus"/> <see cref="GameObject"/> to the depot
        /// </summary>
        public void ReturnBus()
        {
            if (bussesInDepot.Count == 0)
            {
                Debug.Log("The " + CompanyName + " Company has no busses in service.");
                return;
            }

            GameObject bus = bussesOnRoad[0];
            bussesInDepot.Add(bus);
            bussesOnRoad.Remove(bus);

            bus.GetComponent<Bus>().EndService();
        }

        public void Awake()
        {
            // Populate the bus pool
            foreach (Bus bus in GetComponentsInChildren<Bus>())
            {
                bussesOnRoad.Add(bus.gameObject);
            }
        }

        private void OnValidate()
        {
            BusTimetable.ValidateAllBusTimetables();
            name = companyName + " Bus Company";
        }
    }
}
