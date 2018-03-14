using AaronMeaney.BusStop.API.Action;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Responsible for the bus's pathfinding and movement.
    /// Does not contain <see cref="Bus"/> configuration date.
    /// </summary>
    [RequireComponent(typeof(TransformLocationProvider))]
    public class BusDriver : MonoBehaviour
    {
        #region Class Variables

        /// <summary>
        /// Mode changes how the bus will get to its destination.
        /// </summary>
        public enum BusDriverMode
        {
            /// <summary>
            /// The user clicks to make the bus drive to its destination.
            /// </summary>
            Debug,
            /// <summary>
            /// The bus follows its set path.
            /// </summary>
            Path
        }
        
        private AbstractMap map = null;
        /// <summary>
        /// The map that the bus is driving on.
        /// </summary>
        public AbstractMap Map
        {
            get
            {
                if (map == null)
                {
                    map = FindObjectOfType<AbstractMap>();
                }

                return map;
            }
        }

        [SerializeField]
        private float speed;
        /// <summary>
        /// The bus speed.
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        // TODO: Remove isDriving when implementing hailing
        private bool isDriving = true;
        /// <summary>
        /// If the bus is driving/stopped.
        /// </summary>
        public bool IsDriving
        {
            get { return isDriving; }
            set { isDriving = value; }
        }
        
        [SerializeField]
        private BusDriverMode driverMode;
        /// <summary>
        /// The current driver mode of the bus.
        /// </summary>
        public BusDriverMode DriverMode
        {
            get { return driverMode; }
        }

        [SerializeField]
        private Vector3 currentDestination;
        /// <summary>
        /// The location that the bus is driving towards.
        /// </summary>
        public Vector3 CurrentDestination
        {
            get { return currentDestination; }
            set { currentDestination = value; }
        }

        private BusPathfinder currentBusPath;
        /// <summary>
        /// The path currently assigned to the bus.
        /// </summary>
        public BusPathfinder CurrentBusPath
        {
            get { return currentBusPath; }
            set
            {
                currentBusPathNode = 0;
                currentBusPath = value;
                CurrentDestination = transform.position;
                GetComponent<BusPathfinderVisualiser>().ClearVisualisation();

                if (GetComponent<BusPathfinderVisualiser>())
                {
                    CurrentBusPath.onBusPathPopulated += GetComponent<BusPathfinderVisualiser>().SetBusPathVisualisation;
                }
                
                if (GetComponent<PostBusRoute>())
                {
                    CurrentBusPath.onBusPathPopulated += GetComponent<PostBusRoute>().PerformPost;
                }
            }
        }

        private int currentBusPathNode = 0;
        #endregion
        
        /// <summary>
        /// Start() instead of Awake() since <see cref="BusCompany"/> will set <see cref="GameObject.SetActive(false)"/> on Awake
        /// </summary>
        private void Start()
        {
            CurrentBusPath = new BusPathfinder();
        }

        private void Update()
        {
            // Rotate and Drive towards immediate destination if the bus is not there yet
            if (GetDistanceFromDestination() > 1)
            {
                if (isDriving)
                {
                    RotateTowardsDestination();
                    DriveForward();
                }
            }
            else if (DriverMode.Equals(BusDriverMode.Path))
            {
                // Update the current destination if the mode is set to path
                if (currentBusPathNode < currentBusPath.Size)
                {
                    CurrentDestination = currentBusPath.CoordinateLocations[currentBusPathNode].AsUnityPosition(map);
                    currentBusPathNode++;
                }
            }
        }

        /// <summary>
        /// Drives in a straight line to the final destination.
        /// </summary>
        private void DriveDebugMode()
        {
            if (isDriving && GetDistanceFromDestination() > 1)
            {
                RotateTowardsDestination();
                DriveForward();
            }
        }

        /// <summary>
        /// Drives along a path to the final destination.
        /// </summary>
        private void DrivePathMode()
        {
            if (isDriving && GetDistanceFromDestination() > 1 && CurrentBusPath.State == BusPathfinder.PathfinderState.Ready)
            {
                RotateTowardsDestination();
                DriveForward();
            }
            else if (currentBusPathNode < currentBusPath.Size)
            {
                CurrentDestination = currentBusPath.CoordinateLocations[currentBusPathNode].AsUnityPosition(map);
                currentBusPathNode++;
            }
        }

        /// <summary>
        /// Returns the distance from the current destination.
        /// </summary>
        private float GetDistanceFromDestination()
        {
            return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentDestination.x, currentDestination.z));
        }

        /// <summary>
        /// Makes the vehicle rotate towards the destination.
        /// </summary>
        private void RotateTowardsDestination()
        {
            transform.LookAt(currentDestination);
        }

        /// <summary>
        /// Drives the vehicle forward by translating its transform position forward.
        /// </summary>
        private void DriveForward()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}
