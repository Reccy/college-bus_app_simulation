using Rotorz.ReorderableList;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents the list of <see cref="RouteWaypoint"/>s in a <see cref="BusRoute"/>.
    /// Does NOT contain any timetable/time slot data.
    /// </summary>
    public class BusRoute : MonoBehaviour
    {
        [SerializeField]
        private string routeId;
        /// <summary>
        /// The ID of this Route
        /// </summary>
        public string RouteId { get { return routeId; } }

        [SerializeField]
        [HideInInspector]
        private Color gizmoColor;

        [SerializeField]
        [HideInInspector]
        private bool gizmoEnabled;
        
        [SerializeField]
        [HideInInspector]
        private List<RouteWaypoint> routeWaypoints;
        /// <summary>
        /// The list of <see cref="RouteWaypoint"/>s in the <see cref="BusRoute"/> 
        /// </summary>
        public List<RouteWaypoint> RouteWaypoints { get { return routeWaypoints; } }
        
        private void OnDrawGizmos()
        {
            if (gizmoEnabled)
            {
                Gizmos.color = gizmoColor;

                for (int i = 1; i < routeWaypoints.Count; i++)
                {
                    if (routeWaypoints[i - 1] != null && routeWaypoints[i] != null)
                    {
                        Vector3 dir = routeWaypoints[i].transform.position - routeWaypoints[i - 1].transform.position;
                        DrawArrow.ForGizmo(routeWaypoints[i - 1].transform.position, dir, Gizmos.color, 0.4f, arrowPosition: 0.5f);
                    }
                }
            }
        }

        private void OnValidate()
        {
            name = "Bus Route " + RouteId;
        }
    }

    [CustomEditor(typeof(BusRoute))]
    public class BusRouteEditor : Editor
    {
        bool isGizmoEditorShown = true;
        private SerializedProperty _gizmoColor;
        private SerializedProperty _gizmoEnabled;
        private SerializedProperty _routeId;
        private SerializedProperty _routeWaypoints;

        private void OnEnable()
        {
            _gizmoColor = serializedObject.FindProperty("gizmoColor");
            _gizmoEnabled = serializedObject.FindProperty("gizmoEnabled");
            _routeId = serializedObject.FindProperty("routeId");
            _routeWaypoints = serializedObject.FindProperty("routeWaypoints");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();
            
            // Waypoint List
            ReorderableListGUI.Title(typeof(RouteWaypoint).Name + " List");
            ReorderableListGUI.ListField(_routeWaypoints);

            // Gizmos
            isGizmoEditorShown = EditorGUILayout.Foldout(isGizmoEditorShown, "Gizmo Visualisation");

            if (isGizmoEditorShown)
            {
                _gizmoEnabled.boolValue = EditorGUILayout.Toggle("Gizmo Enabled", _gizmoEnabled.boolValue);
                _gizmoColor.colorValue = EditorGUILayout.ColorField("Gizmo Color", _gizmoColor.colorValue);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
