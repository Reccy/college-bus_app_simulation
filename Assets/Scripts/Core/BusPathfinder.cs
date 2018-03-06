﻿using System.Collections.Generic;
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

        private List<CoordinateLocation> coordinateLocations = new List<CoordinateLocation>();
        public List<CoordinateLocation> CoordinateLocations
        {
            get { return coordinateLocations; }
        }

        public int Size
        {
            get { return coordinateLocations.Count; }
        }

        private bool isReady = false;
        /// <summary>
        /// True if <see cref="BusPathfinder"/> is ready to be driven along.
        /// False if not ready, e.g. missing nodes.
        /// </summary>
        public bool IsReady
        {
            get { return isReady; }
            set { isReady = value; }
        }

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
            isReady = false;
            
            // Query the Mapbox Directions API, the response is handled by the HandleDirectionsResponse method
            directions = MapboxAccess.Instance.Directions;
            DirectionResource directionResource = new DirectionResource(positions, RoutingProfile.Driving);
            directionResource.Steps = true;
            directions.Query(directionResource, HandleDirectionsResponse);
        }

        /// <summary>
        /// Handles the response called from <see cref="SetDirectionsToPosition(AbstractMap, Vector2d[])"/> by populating the <see cref="CoordinateLocation"/>
        /// </summary>
        private void HandleDirectionsResponse(DirectionsResponse response)
        {
            List<CoordinateLocation> newNodes = new List<CoordinateLocation>();
            
            // Welcome to nesting hell
            //  <Please>
            //      <Enjoy>
            //          <Your>
            //              <Stay>
            //              <Stay/>
            //          <Your/>
            //      <Enjoy/>
            //  <Please/>
            //
            // Seriously though, this just gets all the points returned from the Mapbox API
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

            CoordinateLocations.Clear();
            CoordinateLocations.AddRange(newNodes);

            isReady = true;

            if (onBusPathPopulated != null)
                onBusPathPopulated(this);
        }
    }
}
