using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        public List<Bus> Busses
        {
            get
            {
                List<Bus> busses = new List<Bus>();

                foreach (Bus bus in bussesInDepot)
                {
                    busses.Add(bus);
                }

                foreach (Bus bus in bussesOnRoad)
                {
                    busses.Add(bus);
                }

                busses.Sort((x, y) => x.name.CompareTo(y.name));

                return busses;
            }
        }
        
        private List<Bus> bussesInDepot = new List<Bus>();
        /// <summary>
        /// List of <see cref="Bus"/> that are ready to be taken from the depot.
        /// </summary>
        public List<Bus> BussesInDepot { get { return bussesInDepot; } }
        
        private List<Bus> bussesOnRoad = new List<Bus>();
        /// <summary>
        /// List of <see cref="Bus"/> that are currently out of the depot.
        /// </summary>
        public List<Bus> BussesOnRoad { get { return bussesOnRoad; } }

        /// <summary>
        /// Deploys an instance of a <see cref="Bus"/> <see cref="GameObject"/> from the depot to a specific <see cref="BusService"/>
        /// </summary>
        /// <param name="service">The <see cref="BusService"/> to assign the bus to
        public Bus DeployBus(BusService service)
        {
            if (bussesInDepot.Count == 0)
            {
                Debug.Log("The " + CompanyName + " Company depot is empty.");
                return null;
            }

            Bus bus = bussesInDepot[0];
            bussesOnRoad.Add(bus);
            bussesInDepot.Remove(bus);

            // Deploy the bus to the service once the route is generated
            service.ServicedBusRoute.OnBusPathReady += () =>
            {
                Debug.Log("Deploying Bus " + bus.RegistrationNumber + " to Route: " + service.ServicedBusRoute.RouteIdInternal);
                bus.GetComponent<Bus>().StartService(service);
            };

            service.ServicedBusRoute.SetPathWaypoints();
            
            return bus;
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

            Bus bus = bussesOnRoad[0];
            bussesInDepot.Add(bus);
            bussesOnRoad.Remove(bus);

            bus.GetComponent<Bus>().EndService();
        }

        private void Awake()
        {
            // Populate the bus pool
            foreach (Bus bus in transform.Find("Bus Pool").GetComponentsInChildren<Bus>())
            {
                bussesInDepot.Add(bus);
                bus.gameObject.SetActive(false);
            }
        }

        private void OnValidate()
        {
            BusTimetable.ValidateAllBusTimetables();
            name = companyName + " Bus Company";
        }
    }

    [CustomEditor(typeof(BusCompany))]
    public class BusCompanyEditor : Editor
    {
        int selectedBusIndex;
        Bus selectedBus;
        BusCompany company;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (Application.isPlaying)
            {
                company = ((BusCompany)target);

                EditorGUILayout.Space();
                BusSelectionPopup();
                BusDepotButtons();
            }
        }

        /// <summary>
        /// Popup to allow user to select the Bus
        /// </summary>
        private void BusSelectionPopup()
        {
            EditorGUILayout.LabelField("Select Bus", EditorStyles.boldLabel);

            if (selectedBusIndex >= GetCompanyBussesAsStrings().Count)
            {
                selectedBusIndex = 0;
            }

            selectedBusIndex = EditorGUILayout.Popup(selectedBusIndex, GetCompanyBussesAsStrings().ToArray());
            selectedBus = company.Busses[selectedBusIndex];
        }

        /// <summary>
        /// Buttons and handles for the Bus Depot
        /// </summary>
        private void BusDepotButtons()
        {
            if (company.BussesInDepot.Contains(selectedBus))
            {
                if (GUILayout.Button("Start Random Service"))
                {
                    BusTimetable randomTimetable = company.BusTimetables[Random.Range(0, company.BusTimetables.Count - 1)];
                    BusService randomService = randomTimetable.BusServices[Random.Range(0, randomTimetable.BusServices.Count - 1)];

                    selectedBus.StartService(randomService);
                }
            }
            else if (company.BussesOnRoad.Contains(selectedBus))
            {
                if (GUILayout.Button("End Service"))
                {
                    selectedBus.EndService();
                }
            }
            else
            {
                Debug.LogError("Invalid Bus Selected in " + company.name + " depot: " + selectedBusIndex);

                selectedBusIndex = 0;
                selectedBus = null;
            }
        }

        private List<string> GetCompanyBussesAsStrings()
        {
            List<string> busNames = new List<string>();

            foreach (Bus bus in ((BusCompany)target).Busses)
            {
                busNames.Add(bus.name);
            }

            if (busNames.Count == 0)
            {
                return new List<string> { "None" };
            }

            return busNames;
        }
    }
}
