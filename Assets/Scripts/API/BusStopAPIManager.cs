using UnityEngine;

namespace AaronMeaney.BusStop.API
{
    public class BusStopAPIManager : MonoBehaviour
    {
        [SerializeField]
        private BusStopAPIConfiguration[] configurations;
        /// <summary>
        /// The potential configurations for the Bus Stop API.
        /// </summary>
        public BusStopAPIConfiguration[] Configurations
        {
            get { return configurations; }
        }
        
        [SerializeField]
        private int selectedConfigurationIndex;
        /// <summary>
        /// The index of the selected configuration.
        /// </summary>
        public int SelectedConfigurationIndex
        {
            get { return selectedConfigurationIndex; }
            set { selectedConfigurationIndex = value; }
        }
        
        [SerializeField]
        private BusStopAPIConfiguration selectedConfiguration;
        /// <summary>
        /// The configuration used at the moment.
        /// </summary>
        public BusStopAPIConfiguration SelectedConfiguration
        {
            get { return Configurations[selectedConfigurationIndex]; }
        }
        
        /// <summary>
        /// Used to update the inspector.
        /// </summary>
        private void OnValidate()
        {
            if (SelectedConfigurationIndex >= 0 && SelectedConfigurationIndex < Configurations.Length)
            { 
                selectedConfiguration = Configurations[SelectedConfigurationIndex];
            }
            else
            {
                SelectedConfigurationIndex = 0;

                if (Configurations.Length > 0)
                    selectedConfiguration = Configurations[SelectedConfigurationIndex];
            }
        }
    }
}
