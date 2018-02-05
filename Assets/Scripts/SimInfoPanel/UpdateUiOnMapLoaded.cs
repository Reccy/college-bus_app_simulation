using UnityEngine;
using Mapbox.Unity.Map;
using System.Collections.Generic;

namespace AaronMeaney.BusStop.UI
{
    /// <summary>
    /// Swaps visible/invisible UI elements when the map is loaded.
    /// </summary>
    public class UpdateUiOnMapLoaded : MonoBehaviour
    {
        // Copied from Mapbox example code:
        // Mapbox\Examples\Scripts\LoadingPanelController.cs

        [SerializeField]
        private MapVisualizer mapVisualizer;
        /// <summary>
        /// The Map Visualiser to listen for the state change.
        /// </summary>
        public MapVisualizer MapVisualizer
        {
            get { return mapVisualizer; }
            set { mapVisualizer = value; }
        }

        [SerializeField]
        private List<GameObject> setInvisible;
        /// <summary>
        /// The object to set invisible when the map is finished loading.
        /// </summary>
        public List<GameObject> SetInvisible
        {
            get { return setInvisible; }
            set { setInvisible = value; }
        }

        [SerializeField]
        private List<GameObject> setVisible;
        /// <summary>
        /// The object to set visible when the map is finished loading.
        /// </summary>
        public List<GameObject> SetVisible
        {
            get { return setVisible; }
            set { setVisible = value; }
        }

        void Awake()
        {
            MapVisualizer.OnMapVisualizerStateChanged += (s) =>
            {
                if (s == ModuleState.Finished)
                {
                    foreach (GameObject invisible in SetInvisible) {
                        invisible.SetActive(false);
                    }

                    foreach (GameObject visible in SetVisible)
                    {
                        visible.SetActive(true);
                    }
                }
                else if (s == ModuleState.Working)
                {
                    foreach (GameObject invisible in SetInvisible)
                    {
                        invisible.SetActive(true);
                    }
                    
                    foreach (GameObject visible in SetVisible)
                    {
                        visible.SetActive(false);
                    }
                }
            };
        }
    }
}
