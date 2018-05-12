using Mapbox.Unity.Map;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a waypoint along a <see cref="BusPathfinder"/>
    /// </summary>
    [ExecuteInEditMode] // For Editor Validation
    [RequireComponent(typeof(SnapToTerrain), typeof(PlaceAtCoordinates))]
    public class RouteWaypoint : MonoBehaviour
    {
        private SnapToTerrain snapToTerrain;
        private AbstractMap map;

        private PlaceAtCoordinates placeAtCoordinates;
        public PlaceAtCoordinates PlactAtCoordinates { get { return placeAtCoordinates; } }

        public double Latitude { get { return placeAtCoordinates.Latitude; } }
        public double Longitude { get { return placeAtCoordinates.Longitude; } }
        
        public GameObject busStopPrefab = null;

        /// <summary>
        /// The <see cref="BusStop"/> belonging to this <see cref="RouteWaypoint"/>
        /// </summary>
        public BusStop LinkedBusStop
        { get { return GetComponentInChildren<BusStop>(); } }

        /// <summary>
        /// Adds a <see cref="BusStop"/> object to this <see cref="RouteWaypoint"/>.
        /// Only works in edit mode.
        /// </summary>
        public void CreateBusStop()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if (busStopPrefab == null)
                {
                    Debug.LogError("Bus Stop Prefab is null. Can't create Bus Stop!", this);
                    return;
                }

                GameObject newBusStop = (GameObject)PrefabUtility.InstantiatePrefab(busStopPrefab);
                newBusStop.transform.parent = transform;
                newBusStop.transform.position = transform.position;
            }
            else
            {
                Debug.LogError("Bus Stops can only be created in Unity Engine Edit Mode!", this);
            }
        }

        private void Awake()
        {
            snapToTerrain = GetComponent<SnapToTerrain>();
            placeAtCoordinates = GetComponent<PlaceAtCoordinates>();
            map = FindObjectOfType<AbstractMap>();
            
            if (Application.isPlaying)
            {
                map.MapVisualizer.OnMapVisualizerStateChanged += OnMapVisualizerStateChanged;
            }
        }

        /// <summary>
        /// Handles what to do when the <see cref="MapVisualizer"/> state changes
        /// </summary>
        /// <param name="state">The new <see cref="ModuleState"/></param>
        private void OnMapVisualizerStateChanged(ModuleState state)
        {
            if (state == ModuleState.Finished)
            {
                placeAtCoordinates.Execute();
                
                snapToTerrain.PerformSnap();
            }
        }

        #region Editor Validation

        private void OnTransformParentChanged()
        {
            ValidateAllWaypoints();
        }

        /// <summary>
        /// Calls <see cref="UpdateUniqueName"/> for all instances of <see cref="RouteWaypoint"/> and ensures that they are in a "Routes" game object in the hierarchy.
        /// </summary>
        protected internal static void ValidateAllWaypoints()
        {
            foreach (RouteWaypoint routeWaypoint in GameObject.FindObjectsOfType<RouteWaypoint>())
            {
                routeWaypoint.UpdateUniqueName();
                routeWaypoint.ValidateHierarchyPosition();
            }
        }

        /// <summary>
        /// Sets the <see cref="RouteWaypoint"/> name to "RouteWaypoint_" and the result of <see cref="GetInstanceNumber"/>.
        /// If there is a <see cref="LinkedBusStop"/>, then its set to the <see cref="BusStop"/> name.
        /// </summary>
        private void UpdateUniqueName()
        {
            if (LinkedBusStop != null)
            {
                name = LinkedBusStop.BusStopIdInternal + "_Waypoint";
            }
            else
            {
                name = "RouteWaypoint_" + GetInstanceNumber();
            }
        }

        /// <summary>
        /// Ensures that this object's parent is the RouteWaypointContainer.
        /// If it doesn't exist, it creates it and then makes it the parent.
        /// </summary>
        private void ValidateHierarchyPosition()
        {
            GameObject container = GameObject.Find("RouteWaypointContainer");

            if (container != null)
            {
                transform.parent = container.transform;
            }
            else
            {
                container = Instantiate<GameObject>(new GameObject(), Vector3.zero, Quaternion.identity);
                container.name = "RouteWaypointContainer";
                transform.parent = container.transform;
            }
        }
        
        /// <summary>
        /// Returns an number representing what order this object was instantiated at starting from the first instance to the newest instance.
        /// </summary>
        /// <returns>Number representing the order this object was instantiated at.</returns>
        private int GetInstanceNumber()
        {
            int instanceNumber = 0;
            List<RouteWaypoint> routeWaypoints = new List<RouteWaypoint>(FindObjectsOfType<RouteWaypoint>());
            routeWaypoints.Reverse(); // Reverse so that the newest instance is not '1' and is the actual count of 'instances + 1'
            foreach (RouteWaypoint routeWaypoint in routeWaypoints)
            {
                instanceNumber++;
                if (routeWaypoint.Equals(this))
                {
                    return instanceNumber;
                }
            }
            
            throw new MissingReferenceException();
        }
        
        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            // Draw line to Bus Stop
            Gizmos.color = Color.yellow;

            if (LinkedBusStop)
            {
                Gizmos.DrawLine(transform.position, LinkedBusStop.transform.position);
            }

            // Display name of node (Frustum culling source: https://forum.unity.com/threads/handles-label-fail-when-point-behind-camera.79217/ - Barry Northern)
            Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(SceneView.currentDrawingSceneView.camera);
            if (GeometryUtility.TestPlanesAABB(frustum, new Bounds(transform.position, Vector3.one)))
            {
                Handles.Label(transform.position, name);
            }
        }

        #endregion
    }

    [CustomEditor(typeof(RouteWaypoint))]
    public class RouteWaypointEditor : Editor
    {
        private RouteWaypoint routeWaypoint;

        private void OnEnable()
        {
            RouteWaypoint.ValidateAllWaypoints();
        }
        
        private void OnDisable()
        {
            RouteWaypoint.ValidateAllWaypoints();
        }

        public override void OnInspectorGUI()
        {
            routeWaypoint = ((RouteWaypoint)target);

            // Default script field
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RouteWaypoint)target), typeof(RouteWaypoint), false);
            GUI.enabled = true;

            // Bus Stop Creation
            if (routeWaypoint.LinkedBusStop)
            {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.TextField("Linked Bus Stop", routeWaypoint.LinkedBusStop.BusStopIdInternal);

                if (Application.isPlaying)
                {
                    // Get list of all bus passengers
                    List<BusPassenger> busPassengers = new List<BusPassenger>();
                    foreach (Bus bus in FindObjectsOfType<Bus>())
                    {
                        foreach (BusPassenger passenger in bus.Passengers)
                        {
                            if (!busPassengers.Contains(passenger))
                                busPassengers.Add(passenger);
                        }
                    }

                    foreach (BusStop stop in FindObjectsOfType<BusStop>())
                    {
                        foreach (BusPassenger passenger in stop.BusQueue)
                        {
                            if (!busPassengers.Contains(passenger))
                                busPassengers.Add(passenger);
                        }
                    }
                    
                    // Origin Bus Stops
                    List<string> originBusStopsNames = new List<string>();
                    List<BusPassenger> originPassengers = new List<BusPassenger>();
                    foreach (BusPassenger passenger in busPassengers)
                    {
                        if (passenger.DestinationBusStop.Equals(routeWaypoint.LinkedBusStop))
                        {
                            originPassengers.Add(passenger);

                            if (!originBusStopsNames.Contains(passenger.OriginBusStop.BusStopIdInternal))
                            {
                                originBusStopsNames.Add(passenger.OriginBusStop.BusStopId);
                            }
                        }
                    }

                    string originStopsDisplayed = "";
                    foreach (string originStop in originBusStopsNames)
                    {
                        if (originBusStopsNames.IndexOf(originStop) < originBusStopsNames.Count - 1)
                            originStopsDisplayed += originStop + ",\n";
                        else
                            originStopsDisplayed += originStop;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Origin Stops");
                    EditorGUILayout.TextArea(originStopsDisplayed);

                    // Destination Bus Stops
                    List<string> destinationStopsNames = new List<string>();
                    foreach (BusPassenger passenger in routeWaypoint.LinkedBusStop.BusQueue)
                    {
                        if (!destinationStopsNames.Contains(passenger.DestinationBusStop.BusStopIdInternal))
                            destinationStopsNames.Add(passenger.DestinationBusStop.BusStopId);
                    }

                    string destinationStopsDisplayed = "";
                    foreach (string destinationStop in destinationStopsNames)
                    {
                        if (destinationStopsNames.IndexOf(destinationStop) < destinationStopsNames.Count - 1)
                            destinationStopsDisplayed += destinationStop + ",\n";
                        else
                            destinationStopsDisplayed += destinationStop;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Destination Stops");
                    EditorGUILayout.TextArea(destinationStopsDisplayed);
                }   
                
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Clear Passenger Queue"))
                {
                    routeWaypoint.LinkedBusStop.BusQueue.Clear();
                }

                if (GUILayout.Button("Select Bus Stop"))
                {
                    Selection.activeGameObject = routeWaypoint.LinkedBusStop.gameObject;
                }
            }
            else
            {
                if (GUILayout.Button("Create Bus Stop"))
                {
                    routeWaypoint.CreateBusStop();
                };
            }
        }
    }
}
