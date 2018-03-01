using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    [CustomEditor(typeof(BusStop))]
    class BusStopEditor : Editor
    {
        private BusStop busStop;

        public override void OnInspectorGUI()
        {
            busStop = (BusStop)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Update Position"))
                busStop.UpdatePosition();
        }
    }
}
