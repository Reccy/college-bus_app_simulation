using System.Collections.Generic;
using Mapbox.Unity.Utilities;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Directions;
using Mapbox.Unity;
using System;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents the <see cref="BusPathfinder"/> as a list of <see cref="CoordinateLocation"/>s.
    /// </summary>
    [Serializable]
    public class BusPathfinder
    {
        public delegate void OnBusPathPopulated(BusPathfinder thisPath);
        /// <summary>
        /// Delegate is called when the <see cref="BusPathfinder"/> has been fully populated with <see cref="CoordinateLocation"/>.
        /// </summary>
        public OnBusPathPopulated onBusPathPopulated;

        public enum PathfinderState { Ready, Processing, Idle };
        private PathfinderState state = PathfinderState.Idle;
        /// <summary>
        /// The <see cref="PathfinderState"/> of this <see cref="BusPathfinder"/>
        /// </summary>
        public PathfinderState State
        {
            get { return State; }
        }

        private List<CoordinateLocation> coordinateLocations = new List<CoordinateLocation>();
        /// <summary>
        /// <see cref="List{T}"/> of <see cref="CoordinateLocation"/>s in the Path
        /// </summary>
        public List<CoordinateLocation> CoordinateLocations
        {
            get { return coordinateLocations; }
        }

        /// <summary>
        /// Size of <see cref="CoordinateLocations"/>
        /// </summary>
        public int Size
        {
            get { return coordinateLocations.Count; }
        }

        /// <summary>
        /// <see cref="List{T}"/> of <see cref="CoordinateLocation"/>s that will replace <see cref="CoordinateLocations"/>
        /// </summary>
        private List<CoordinateLocation> newNodes = new List<CoordinateLocation>();

        /// <summary>
        /// Queue of queries for the Mapbox API
        /// </summary>
        private Queue<DirectionResource> queryQueue = new Queue<DirectionResource>();
        
        Directions directions;

        /// <summary>
        /// Replaces the <see cref="BusPathfinder"/> nodes with <see cref="Directions"/>.
        /// The position is translated to coordinates and then the directions are generated.
        /// </summary>
        /// <param name="fromPosition">The world space position to begin navigating from.</param>
        /// <param name="toPosition">The world space position to navigate to.</param>
        public void SetDirectionsToPosition(AbstractMap map, Vector3 fromPosition, Vector3 toPosition)
        {
            // Get the geo positions from the world Vector3 positions
            Vector2d fromGeoPosition = fromPosition.GetGeoPosition(map.CenterMercator, map.WorldRelativeScale);
            Vector2d toGeoPosition = toPosition.GetGeoPosition(map.CenterMercator, map.WorldRelativeScale);

            SetDirectionsToPosition(map, new Vector2d[] { fromGeoPosition, toGeoPosition });
        }

        /// <summary>
        /// Replaces the <see cref="BusPathfinder"/> nodes with <see cref="Directions"/>.
        /// The position is translated to coordinates and then the directions are generated.
        /// </summary>
        /// <param name="positions">Array of positions to navigate through.</param>
        public void SetDirectionsToPosition(AbstractMap map, Vector2d[] positions)
        {
            directions = MapboxAccess.Instance.Directions;

            newNodes = new List<CoordinateLocation>();
            
            for (int i = 0; i < positions.Length; i += 24)
            {
                Vector2d[] positionFragment = new List<Vector2d>(positions).GetRange(i, Math.Min(positions.Length - i, 25)).ToArray();
                
                DirectionResource directionResource = new DirectionResource(positionFragment, RoutingProfile.Driving);
                directionResource.Steps = true;
                queryQueue.Enqueue(directionResource);
            }

            PerformQuery();
        }

        /// <summary>
        /// Performs a Query from the <see cref="queryQueue"/>
        /// </summary>
        private void PerformQuery()
        {
            state = PathfinderState.Processing;
            directions.Query(queryQueue.Dequeue(), HandleDirectionsResponse);
        }

        /// <summary>
        /// Handles the response called from <see cref="SetDirectionsToPosition(AbstractMap, Vector2d[])"/> by populating the <see cref="CoordinateLocation"/>
        /// </summary>
        private void HandleDirectionsResponse(DirectionsResponse response)
        {
            // Gets all the points returned from the Mapbox API
            foreach (Route route in response.Routes)
            {
                foreach (Leg leg in route.Legs)
                {
                    foreach (Step step in leg.Steps)
                    {
                        foreach (Vector2d point in step.Geometry)
                        {
                            newNodes.Add(new CoordinateLocation((float)point.x, (float)point.y));
                        }
                    }
                }
            }
            
            if (queryQueue.Count > 0)
            {
                PerformQuery();
            }
            else
            {
                state = PathfinderState.Ready;

                CoordinateLocations.Clear();
                CoordinateLocations.AddRange(newNodes);

                if (onBusPathPopulated != null)
                    onBusPathPopulated(this);
            }
        }
    }
}
