using UnityEngine;

namespace AaronMeaney.BusStop.Utilities
{
    /// <summary>
    /// Non-functional script to allow notes to be written in the inspector window
    /// </summary>
    public class InspectorNote : MonoBehaviour
    {
        [Multiline]
        public string note;
    }

}
