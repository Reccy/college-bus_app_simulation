using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.Utilities
{
    public static class ExtensionMethods
    {
        public static long UnixTime(this System.DateTime datetime)
        {
            System.TimeSpan timeSpan = (datetime - new System.DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
    }
}
