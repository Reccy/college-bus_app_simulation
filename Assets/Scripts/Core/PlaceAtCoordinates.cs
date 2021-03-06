﻿using Mapbox.Unity.Map;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Places this <see cref="GameObject"/> at the desired <see cref="CoordinateLocation"/>
    /// </summary>
    public class PlaceAtCoordinates : MonoBehaviour
    {
        /// <summary>
        /// Reference to the <see cref="AbstractMap"/>. Set by script on <see cref="Awake"/>.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private AbstractMap map;

        /// <summary>
        /// The <see cref="CoordinateLocation"/> to place this <see cref="GameObject"/> at.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private CoordinateLocation coordinateLocation;
        public CoordinateLocation CoordinateLocation {
            get { return coordinateLocation; }
            set { coordinateLocation = value; }
        }

        /// <summary>
        /// The Latitude of the <see cref="CoordinateLocation"/>
        /// </summary>
        public double Latitude { get { return CoordinateLocation.Latitude; } }

        /// <summary>
        /// The Longitude of the <see cref="CoordinateLocation"/>
        /// </summary>
        public double Longitude { get { return CoordinateLocation.Longitude; } }

        [SerializeField]
        public bool executeOnAwake;

        private bool finishedPlacement = false;
        /// <summary>
        /// If the <see cref="Execute"/> method has yet been called on this instance
        /// </summary>
        public bool FinishedPlacement { get { return finishedPlacement; } }

        /// <summary>
        /// Called when <see cref="Execute"/> is finished
        /// </summary>
        public Action OnPlacementFinished;
        
        private void Awake()
        {
            map = FindObjectOfType<AbstractMap>();

            if (executeOnAwake)
                Execute();
        }

        /// <summary>
        /// Places the <see cref="GameObject"/> at the <see cref="CoordinateLocation"/>
        /// </summary>
        public void Execute()
        {
            if (map == null)
            {
                Debug.LogError("Can't execute PlaceAtCoordinates on " + name + ". Map is null.");
                return;
            };

            transform.position = coordinateLocation.AsUnityPosition(map);

            finishedPlacement = true;

            if (OnPlacementFinished != null)
                OnPlacementFinished();
        }

    }

    [CustomEditor(typeof(PlaceAtCoordinates))]
    public class PlaceAtCoordinatesEditor : Editor
    {
        private SerializedProperty _latitude;
        private SerializedProperty _longitude;
        private SerializedProperty _executeOnAwake;
        private SerializedProperty _map;

        private void OnEnable()
        {
            _latitude = serializedObject.FindProperty("coordinateLocation").FindPropertyRelative("latitude");
            _longitude = serializedObject.FindProperty("coordinateLocation").FindPropertyRelative("longitude");
            _executeOnAwake = serializedObject.FindProperty("executeOnAwake");
            _map = serializedObject.FindProperty("map");
        }

        public override void OnInspectorGUI()
        {
            // Default script field
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((PlaceAtCoordinates)target), typeof(PlaceAtCoordinates), false);
            GUI.enabled = true;

            // Help Box
            EditorGUILayout.HelpBox("Places the object at the transform position of the coordinates when OnAwake() is called.", MessageType.Info);
            
            serializedObject.Update();

            // Execute on awake
            _executeOnAwake.boolValue = GUILayout.Toggle(_executeOnAwake.boolValue, "Execute on Awake");

            // Coordinates
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Coordinates", "The latitude and longitude the place the object at"), EditorStyles.boldLabel);
            _latitude.floatValue = EditorGUILayout.FloatField("Latitude", _latitude.floatValue);
            _longitude.floatValue = EditorGUILayout.FloatField("Longitude", _longitude.floatValue);

            serializedObject.ApplyModifiedProperties();
            
            // Manual execution
            if (_map.objectReferenceValue != null)
            {
                if (GUILayout.Button("Place At Coordinates"))
                {
                    ((PlaceAtCoordinates)target).Execute();
                }
            }
        }
    }
}
