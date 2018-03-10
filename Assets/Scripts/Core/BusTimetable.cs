using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a standard <see cref="BusTimetable"/>, containing multiple <see cref="BusRoute"/>s with added scheduling data.
    /// </summary>
    [System.Serializable]
    public class BusTimetable : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="BusCompany"/> that this <see cref="BusTimetable"/> belongs to.
        /// </summary>
        public BusCompany ParentBusCompany {
            get
            {
                if (GetComponentInParent<BusCompany>())
                    return GetComponentInParent<BusCompany>();
                
                return null;
            }
        }

        /// <summary>
        /// The name of the <see cref="BusCompany"/> that owns this <see cref="BusTimetable"/>
        /// </summary>
        public string CompanyName
        {
            get
            {
                if (ParentBusCompany)
                    return ParentBusCompany.CompanyName;

                return null;
            }
        }

        [SerializeField]
        private string timetableName;
        /// <summary>
        /// The name of this <see cref="BusTimetable"/>. For example, "Weekday", "Weekends", "Holidays".
        /// </summary>
        public string TimetableName { get { return timetableName; } }

        [SerializeField]
        private List<BusService> busServices;
        /// <summary>
        /// The <see cref="BusServices"/>s running on this <see cref="BusTimetable"/>.
        /// </summary>
        public List<BusService> BusServices {
            get
            {
                if (busServices == null)
                    busServices = new List<BusService>();

                return busServices;
            }
        }
        
        #region Service Days

        [SerializeField]
        private bool mondayService = false;

        [SerializeField]
        private bool tuesdayService = false;

        [SerializeField]
        private bool wednesdayService = false;

        [SerializeField]
        private bool thursdayService = false;

        [SerializeField]
        private bool fridayService = false;

        [SerializeField]
        private bool saturdayService = false;

        [SerializeField]
        private bool sundayService = false;

        public bool RunningOnDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return mondayService;
                case DayOfWeek.Tuesday:
                    return tuesdayService;
                case DayOfWeek.Wednesday:
                    return wednesdayService;
                case DayOfWeek.Thursday:
                    return thursdayService;
                case DayOfWeek.Friday:
                    return fridayService;
                case DayOfWeek.Saturday:
                    return saturdayService;
                case DayOfWeek.Sunday:
                    return sundayService;
            }
            throw new ArgumentException();
        }

        #endregion
        
        #region Editor Validation
        private void OnValidate()
        {
            ValidateAllBusTimetables();
        }

        /// <summary>
        /// Ensures that all <see cref="BusTimetable"/>s belong to a <see cref="ParentBusCompany"/>
        /// </summary>
        public static void ValidateAllBusTimetables()
        {
            List<BusTimetable> allBusTimetables = new List<BusTimetable>(FindObjectsOfType<BusTimetable>());

            foreach (BusTimetable timetable in allBusTimetables)
            {
                if (timetable.CompanyName == null)
                {
                    timetable.name = "BAD_COMPANY Timetable [" + timetable.TimetableName + "]";
                    Debug.LogError("Bus Timetable has no parent Bus Company: " + timetable.TimetableName, timetable);

                }
                else
                {
                    timetable.name = timetable.CompanyName + " Timetable [" + timetable.TimetableName + "]";
                }
            }
        }
        #endregion
    }

    [CustomEditor(typeof(BusTimetable))]
    public class BusTimetableEditor : Editor
    {
        private SerializedProperty _timetableName;

        // Service Days
        private SerializedProperty _mondayService;
        private SerializedProperty _tuesdayService;
        private SerializedProperty _wednesdayService;
        private SerializedProperty _thursdayService;
        private SerializedProperty _fridayService;
        private SerializedProperty _saturdayService;
        private SerializedProperty _sundayService;
        
        private void OnEnable()
        {
            BusTimetable.ValidateAllBusTimetables();

            _timetableName = serializedObject.FindProperty("timetableName");

            _mondayService = serializedObject.FindProperty("mondayService");
            _tuesdayService = serializedObject.FindProperty("tuesdayService");
            _wednesdayService = serializedObject.FindProperty("wednesdayService");
            _thursdayService = serializedObject.FindProperty("thursdayService");
            _fridayService = serializedObject.FindProperty("fridayService");
            _saturdayService = serializedObject.FindProperty("saturdayService");
            _sundayService = serializedObject.FindProperty("sundayService");
        }

        public override void OnInspectorGUI()
        {
            // Default script field
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((BusTimetable)target), typeof(BusTimetable), false);
            GUI.enabled = true;

            serializedObject.Update();

            // Timetable Name
            _timetableName.stringValue = EditorGUILayout.TextField("Timetable Name", _timetableName.stringValue);

            // Service Days
            _mondayService.boolValue = EditorGUILayout.Toggle("Runs on Monday", _mondayService.boolValue);
            _tuesdayService.boolValue = EditorGUILayout.Toggle("Runs on Tuesday", _tuesdayService.boolValue);
            _wednesdayService.boolValue = EditorGUILayout.Toggle("Runs on Wednesday", _wednesdayService.boolValue);
            _thursdayService.boolValue = EditorGUILayout.Toggle("Runs on Thursday", _thursdayService.boolValue);
            _fridayService.boolValue = EditorGUILayout.Toggle("Runs on Friday", _fridayService.boolValue);
            _saturdayService.boolValue = EditorGUILayout.Toggle("Runs on Saturday", _saturdayService.boolValue);
            _sundayService.boolValue = EditorGUILayout.Toggle("Runs on Sunday", _sundayService.boolValue);

            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// Editor Window for <see cref="BusTimetable"/>.
    /// Some of the worst code I've ever written.
    /// Unity's Serialization is a trash fire.
    /// Forgive me.
    /// </summary>
    [ExecuteInEditMode]
    public class BusTimetableEditorWindow : EditorWindow
    {
        #region Configuration
        private BusTimetable timetable = null;
        private enum BusTimetableConfigMode { NORMAL, EDIT, REMOVE }
        private BusTimetableConfigMode ConfigMode = BusTimetableConfigMode.NORMAL;
        
        [MenuItem("Window/Bus Stop Timetable Editor")]
        public static void Init()
        {
            GetWindow<BusTimetableEditorWindow>();
        }

        private void OnSelectionChange()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.GetComponent<BusTimetable>())
                {
                    timetable = obj.GetComponent<BusTimetable>();
                    break;
                }
            }

            GetWindow<BusTimetableEditorWindow>().Repaint();
        }
        #endregion

        Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            // Title setup
            titleContent.image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Sprites/bus_icon.png");
            titleContent.text = "Timetable";

            // Don't render rest of the UI if no timetable is selected
            if (timetable == null)
            {
                EditorGUILayout.LabelField("No Timetable Selected", EditorStyles.largeLabel);
                return;
            }

            // START
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);
            #region Timetable Header

            // Show the name of the selected timetable
            EditorGUILayout.LabelField(timetable.name, EditorStyles.largeLabel);
            
            // Show what days the timetable runs on
            string daysRunning = "";

            foreach(DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (timetable.RunningOnDay(day))
                {
                    if (daysRunning != "")
                        daysRunning += ", ";

                    daysRunning += day.ToString();
                }
            }

            if (daysRunning == "")
            {
                daysRunning = "Not running on any days.";
            }
            else
            {
                daysRunning = "Service Days: " + daysRunning;
            }

            EditorGUILayout.LabelField(daysRunning);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            #endregion

            #region Timetable Body
            SerializedObject _timetable = new SerializedObject(timetable);
            List<BusService> companyServices = timetable.BusServices;
            List<BusRoute> companyRoutes = timetable.ParentBusCompany.BusRoutes;
            List<BusRoute> servicedRoutes = new List<BusRoute>();
            List<BusStop> servicedStops = new List<BusStop>();

            // Get list of bus routes being serviced by each bus service
            foreach (BusService service in companyServices)
            {
                if (service.ServicedBusRoute != null && !servicedRoutes.Contains(service.ServicedBusRoute))
                {
                    servicedRoutes.Add(service.ServicedBusRoute);
                }
            }

            // Get list of bus stops being serviced by each route.
            servicedStops = BusRoute.TopologicalSort(servicedRoutes);

            // Render bus stops
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            {
                // Render serviced Bus Stops
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Bus Stops", EditorStyles.boldLabel, GUILayout.Height(18));

                    if (servicedStops.Count == 0)
                        EditorGUILayout.LabelField("None");

                    for (int i = 0; i < servicedStops.Count; i++)
                    {
                        EditorGUILayout.LabelField(servicedStops[i].BusStopId, GUILayout.Height(18));
                    }
                }
                EditorGUILayout.EndVertical();

                // Render each Bus Service
                BusService busServiceToDelete = null;
                foreach (BusService service in companyServices)
                {
                    EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
                    {
                        // Selected Bus Route for Service
                        List<string> companyRoutesAsStrings = new List<string> { "None" };

                        // Need to populate the string list and set the selected index to persist selected bus route
                        int selectedBusRouteIndex = 0;
                        for (int routeIndex = 0; routeIndex < companyRoutes.Count; routeIndex++)
                        {
                            BusRoute route = companyRoutes[routeIndex];

                            companyRoutesAsStrings.Add(route.RouteId);
                            if (route == service.ServicedBusRoute)
                            {
                                selectedBusRouteIndex = routeIndex + 1;
                            }
                        }

                        // Route Dropdown Selection
                        if (ConfigMode == BusTimetableConfigMode.NORMAL)
                        {
                            // Selection Popup
                            int originalSelectedBusRouteIndex = selectedBusRouteIndex;
                            selectedBusRouteIndex = (EditorGUILayout.Popup(selectedBusRouteIndex, companyRoutesAsStrings.ToArray(), GUILayout.Width(60)) - 1);

                            if ((originalSelectedBusRouteIndex - 1) != selectedBusRouteIndex)
                            {
                                Debug.Log("OG: " + originalSelectedBusRouteIndex + " != " + selectedBusRouteIndex);
                                EditorUtility.SetDirty(timetable);
                                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                            }

                            // Apply selection
                            if (selectedBusRouteIndex == -1)
                            {
                                service.ServicedBusRoute = null;
                            }
                            else
                            {
                                service.ServicedBusRoute = companyRoutes[selectedBusRouteIndex];
                            }
                        }
                        
                        // Delete Service Button
                        if (ConfigMode == BusTimetableConfigMode.REMOVE)
                        {
                            if (GUILayout.Button("☒ " + companyRoutesAsStrings[selectedBusRouteIndex], GUILayout.Width(60)))
                            {
                                Debug.Log("Deleting " + companyRoutesAsStrings[selectedBusRouteIndex] + " service");

                                // Schedule this service for deletion
                                busServiceToDelete = service;
                            }
                        }

                        if (service.ServicedBusRoute != null)
                        {
                            // Time Slots
                            for (int stopIndex = 0; stopIndex < servicedStops.Count; stopIndex++)
                            {
                                EditorGUILayout.BeginHorizontal(GUILayout.Width(60));

                                BusStop stop = servicedStops[stopIndex];

                                if (service.ServicedBusRoute.BusStops.Contains(stop))
                                {
                                    // Create a new Time Slot if it doesn't exist
                                    bool hasTimeSlot = false;

                                    foreach (BusTimeSlot slot in service.TimeSlots)
                                    {
                                        if (slot.ScheduledBusStop == stop)
                                        {
                                            hasTimeSlot = true;
                                            break;
                                        }
                                    }

                                    if (!hasTimeSlot)
                                    {
                                        BusTimeSlot slot = new BusTimeSlot();
                                        slot.ScheduledBusStop = stop;
                                        service.TimeSlots.Add(slot);
                                    }

                                    BusTimeSlot timeSlot = null;
                                    
                                    foreach (BusTimeSlot slot in service.TimeSlots)
                                    {
                                        if (slot.ScheduledBusStop == stop)
                                            timeSlot = slot;
                                    }

                                    if (timeSlot == null)
                                    {
                                        Debug.LogError("Something awful happened and you really fucked up this time Aaron.");
                                        return;
                                    }

                                    // Set Scheduled Stop if not already set
                                    if (timeSlot.ScheduledBusStop != stop)
                                    {
                                        timeSlot.ScheduledBusStop = stop;
                                    }

                                    // Get Time Slot for each bus stop in this serviced route
                                    int originalScheduledHour = timeSlot.ScheduledHour;
                                    int originalScheduledMinute = timeSlot.ScheduledMinute;
                                    timeSlot.ScheduledHour = EditorGUILayout.IntField(timeSlot.ScheduledHour, GUILayout.Width(28));
                                    timeSlot.ScheduledMinute = EditorGUILayout.IntField(timeSlot.ScheduledMinute, GUILayout.Width(28));

                                    if (originalScheduledHour != timeSlot.ScheduledHour || originalScheduledMinute != timeSlot.ScheduledMinute)
                                    {
                                        EditorUtility.SetDirty(timetable);
                                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                                    }
                                }
                                else
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.TextField("      Ø", GUILayout.Width(60));
                                    EditorGUI.EndDisabledGroup();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }

                if (busServiceToDelete != null)
                {
                    companyServices.Remove(busServiceToDelete);
                    EditorUtility.SetDirty(timetable);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }

                EditorGUILayout.BeginVertical();

                if (ConfigMode == BusTimetableConfigMode.NORMAL)
                {
                    // Render "Add Service" button
                    if (GUILayout.Button("Add Service", GUILayout.Width(100)))
                    {
                        BusService newService = new BusService(timetable);
                        companyServices.Add(newService);

                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        EditorUtility.SetDirty(timetable);
                    }

                    // Render "Edit Mode" button
                    if (GUILayout.Button("Edit Mode", GUILayout.Width(100)))
                    {
                        ConfigMode = BusTimetableConfigMode.EDIT;
                    }

                    // Render "Remove Mode" button
                    if (GUILayout.Button("Remove Mode", GUILayout.Width(100)))
                    {
                        ConfigMode = BusTimetableConfigMode.REMOVE;
                    }
                }
                else
                {
                    // Render "Normal Mode" button
                    if (GUILayout.Button("Done", GUILayout.Width(100)))
                    {
                        ConfigMode = BusTimetableConfigMode.NORMAL;
                    }
                }

                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            #endregion
            
            // END
            EditorGUILayout.EndScrollView();
        }
    }
}
