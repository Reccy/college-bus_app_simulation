using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    [CustomEditor(typeof(SnapToTerrain))]
    class SnapToTerrainEditor : Editor
    {
        private SnapToTerrain snapToTerrain;

        public override void OnInspectorGUI()
        {
            snapToTerrain = (SnapToTerrain)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Perform Snap"))
                snapToTerrain.PerformSnap();
        }
    }
}
