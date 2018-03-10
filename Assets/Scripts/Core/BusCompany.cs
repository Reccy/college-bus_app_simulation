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

        private void OnValidate()
        {
            BusTimetable.ValidateAllBusTimetables();
            name = companyName + " Bus Company";
        }
    }
}
