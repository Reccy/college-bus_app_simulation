using AaronMeaney.BusStop.API.Action;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Responsible for the bus's pathfinding and movement.
    /// </summary>
    [RequireComponent(typeof(TransformLocationProvider))]
    public class BusDriver : MonoBehaviour
    {
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

        [SerializeField]
        private AbstractMap map;
        /// <summary>
        /// The map that the bus is driving on.
        /// </summary>
        public AbstractMap Map
        {
            get { return map; }
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
        private float busOriginDistanceFromRoad = 0.337f;
        /// <summary>
        /// The distance from the bus's origin to the road on the Y axis.
        /// </summary>
        public float BusOriginDistanceFromRoad
        {
            get { return busOriginDistanceFromRoad; }
            set { busOriginDistanceFromRoad = value; }
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
        private Transform currentDestination;
        /// <summary>
        /// The transform that the bus is driving towards.
        /// </summary>
        public Transform CurrentDestination
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
                CurrentDestination.position = transform.position;
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

        private void Awake()
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
                    CurrentDestination.position = currentBusPath.CoordinateLocations[currentBusPathNode].AsUnityPosition(map);
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
            if (isDriving && GetDistanceFromDestination() > 1 && CurrentBusPath.IsReady)
            {
                RotateTowardsDestination();
                DriveForward();
            }
            else if (currentBusPathNode < currentBusPath.Size)
            {
                CurrentDestination.position = currentBusPath.CoordinateLocations[currentBusPathNode].AsUnityPosition(map);
                currentBusPathNode++;
            }
        }

        /// <summary>
        /// Returns the distance from the current destination.
        /// </summary>
        private float GetDistanceFromDestination()
        {
            return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentDestination.position.x, currentDestination.position.z));
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
