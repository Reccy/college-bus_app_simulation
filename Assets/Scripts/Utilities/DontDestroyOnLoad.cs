using UnityEngine;

namespace AaronMeaney.BusStop.Utilities
{
    /// <summary>
    /// Prevents the destruction of this Game Object on load
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
