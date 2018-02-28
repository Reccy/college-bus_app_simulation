using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Contains Bus Stop data.
    /// </summary>
    public class BusStop : MonoBehaviour
    {
        [SerializeField]
        private string identifier = "DEFAULT100";
        /// <summary>
        /// The unique identifier of the Bus Stop.
        /// </summary>
        public string Identifier
        {
            get { return identifier; }
        }
    }
}
