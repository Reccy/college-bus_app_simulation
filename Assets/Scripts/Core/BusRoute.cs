﻿using Mapbox.Directions;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
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
        [HideInInspector]
        private AbstractMap map;

        [SerializeField]
        private string routeId;
        /// <summary>
        /// The ID of this Route
        /// </summary>
        public string RouteId { get { return routeId; } }
        
        [SerializeField]
        [HideInInspector]
        private List<RouteWaypoint> routeWaypoints;
        /// <summary>
        /// The list of <see cref="RouteWaypoint"/>s in the <see cref="BusRoute"/> 
        /// </summary>
        public List<RouteWaypoint> RouteWaypoints { get { return routeWaypoints; } }
        
        /// <summary>
        /// The list of all <see cref="BusStop"/>s in a <see cref="BusRoute"/>
        /// </summary>
        public List<BusStop> BusStops
        {
            get
            {
                List<BusStop> stops = new List<BusStop>();

                foreach (RouteWaypoint routeWaypoint in RouteWaypoints)
                {
                    if (routeWaypoint.LinkedBusStop != null)
                    {
                        stops.Add(routeWaypoint.LinkedBusStop);
                    }
                }

                return stops;
            }
        }

        /// <summary>
        /// The list of <see cref="CoordinateLocation"/>s that the bus has to follow. Generated by the <see cref="BusPathfinder"/>.
        /// </summary>
        private List<CoordinateLocation> pathWaypoints;

        private void OnEnable()
        {
            pathWaypoints = new List<CoordinateLocation>();
        }

        private void Awake()
        {
            map = FindObjectOfType<AbstractMap>();
        }

        /// <summary>
        /// Gets a <see cref="BusPathfinder"/> for each <see cref="RouteWaypoint"/> pair and appends the <see cref="CoordinateLocation"/>s to <see cref="PathWaypoints"/>.
        /// Logic is finished in <see cref="OnPathWaypointsPopulated(BusPathfinder)"/>
        /// </summary>
        public void SetPathWaypoints()
        {
            BusPathfinder pathfinder = new BusPathfinder();
            pathfinder.onBusPathPopulated += OnPathWaypointsPopulated;

            List<Vector2d> positionList = new List<Vector2d>();

            // Convert each Route Waypoint into a Vector2d position for Mapbox
            foreach (RouteWaypoint waypoint in RouteWaypoints)
            {
                Vector2d waypointAsVector = waypoint.transform.position.GetGeoPosition(map.CenterMercator, map.WorldRelativeScale);
                positionList.Add(waypointAsVector);
            }
            
            Vector2d[] positions = positionList.ToArray();

            pathfinder.SetDirectionsToPosition(map, positions);
        }

        /// <summary>
        /// Handles logic after the <see cref="BusPathfinder"/> has generated the <see cref="PathWaypoints"/>
        /// </summary>
        /// <param name="pathfinder"></param>
        public void OnPathWaypointsPopulated(BusPathfinder pathfinder)
        {
            pathWaypoints.Clear();

            pathWaypoints.AddRange(pathfinder.CoordinateLocations);
        }

        /// <summary>
        /// Sorts the <see cref="BusStop"/>s in the list of <see cref="BusRoute"/>s by using Topological Sort.
        /// </summary>
        /// <param name="busRoutes">List of <see cref="BusRoute"/>s to sort</param>
        /// <returns>An ordered topological list of <see cref="BusStop"/>s from each <see cref="BusRoute"/></returns>
        public static List<BusStop> TopologicalSortRoutes(List<BusRoute> busRoutes)
        {
            List<BusStop> sortedBusStops = new List<BusStop>();
            Dictionary<BusRoute, List<BusStopGraphNode>> singleRouteBusStops = new Dictionary<BusRoute, List<BusStopGraphNode>>();
            List<BusStopGraphNode> routeGraph = new List<BusStopGraphNode>();

            // Create individual LinkedLists for each route
            foreach (BusRoute route in busRoutes)
            {
                singleRouteBusStops[route] = new List<BusStopGraphNode>();

                for (int busStopIndex = route.BusStops.Count - 1; busStopIndex >= 0; busStopIndex--)
                {
                    BusStopGraphNode newNode = new BusStopGraphNode(route.BusStops[busStopIndex]);
                    
                    if (busStopIndex + 1 < route.BusStops.Count)
                    {
                        newNode.NextBusStops.Add(singleRouteBusStops[route][busStopIndex + 1]);
                    }

                    singleRouteBusStops[route].Add(newNode);
                }
            }

            foreach (KeyValuePair<BusRoute, List<BusStopGraphNode>> entry in singleRouteBusStops)
            {
                BusStopGraphNode currentNode = entry.Value[0];

                while (currentNode.NextBusStops.Count != 0)
                {
                    Debug.Log(entry.Key + " --> " + currentNode.BusStop.BusStopId + " --> " + currentNode.NextBusStops[0].BusStop.BusStopId);
                    currentNode = currentNode.NextBusStops[0];
                }
            }

            // Combine LinkedLists into a single graph
            

            return sortedBusStops;
        }

        #region Gizmos
        [SerializeField]
        [HideInInspector]
        private Color gizmoColor;

        [SerializeField]
        [HideInInspector]
        private bool gizmoEnabled;

        private void OnDrawGizmos()
        {
            if (gizmoEnabled)
            {
                DrawGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        private void DrawGizmos()
        {
            // Show arrows between Route Waypoints
            Gizmos.color = gizmoColor;

            for (int i = 1; i < routeWaypoints.Count; i++)
            {
                if (routeWaypoints[i - 1] != null && routeWaypoints[i] != null)
                {
                    Vector3 dir = routeWaypoints[i].transform.position - routeWaypoints[i - 1].transform.position;
                    DrawArrow.ForGizmo(routeWaypoints[i - 1].transform.position, dir, Gizmos.color, 0.4f, arrowPosition: 0.5f);
                }
            }

            // Show arrows between Path Waypoints
            Gizmos.color = Color.blue;

            if (pathWaypoints == null)
                pathWaypoints = new List<CoordinateLocation>();

            for (int i = 1; i < pathWaypoints.Count; i++)
            {
                if (pathWaypoints[i - 1] != null && pathWaypoints[i] != null)
                {
                    Vector3 dir = pathWaypoints[i].AsUnityPosition(map) - pathWaypoints[i - 1].AsUnityPosition(map);
                    Gizmos.DrawWireSphere(pathWaypoints[i - 1].AsUnityPosition(map), 0.2f);
                    DrawArrow.ForGizmo(pathWaypoints[i - 1].AsUnityPosition(map), dir, Gizmos.color, 0.4f, arrowPosition: 0.5f);
                }
            }
        }
        #endregion

        private void OnValidate()
        {
            name = "Bus Route " + RouteId;
        }
    }

    [CustomEditor(typeof(BusRoute))]
    public class BusRouteEditor : Editor
    {
        private bool isGizmoEditorShown = true;
        private SerializedProperty _map;
        private SerializedProperty _gizmoColor;
        private SerializedProperty _gizmoEnabled;
        private SerializedProperty _routeId;
        private SerializedProperty _routeWaypoints;

        private void OnEnable()
        {
            _map = serializedObject.FindProperty("map");
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

            // Show the map route as Gizmos
            if (Application.isPlaying && _map.objectReferenceValue != null)
            {
                if (GUILayout.Button("Show Bus Route"))
                {
                    ((BusRoute)target).SetPathWaypoints();
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
