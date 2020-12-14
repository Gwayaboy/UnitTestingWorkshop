// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingAlgorithm.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the RoutingAlgorithm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using System;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    public abstract class RoutingAlgorithm
    {
        protected static readonly Random RandomGenerator = new Random((int)DateTime.UtcNow.Ticks);
        private Route currentRoute;
        private int currentRouteIndex;

        public Intersection StartTrip { get; set; }

        public Intersection EndTrip { get; set; }

        public void PickRoute(Car car)
        {
            RoutePart currentRoutePart = DiscoveredRoutes.ToRoutePart(car.Location.Road);
            if (this.currentRoute == null)
            {
                this.StartNewRoute(car, currentRoutePart);
            }
            else if ((this.currentRoute.Count - 1) <= this.currentRouteIndex)
            {
                this.StartNewRoute(car, currentRoutePart);
            }
            else
            {
                if (!this.currentRoute.Parts[this.currentRouteIndex + 1].Road.IsFree(0))
                {
                    return;
                }

                RoutePart nextRoutePart = this.currentRoute[++this.currentRouteIndex];
                currentRoutePart.Remove(car);
                nextRoutePart.Add(car);
            }
        }

        protected abstract Route SelectBestRoute(Car car);

        private void StartNewRoute(Car car, RoutePart currentRoutePart)
        {
            this.currentRoute = this.SelectBestRoute(car);
            if (this.currentRoute == null)
            {
                return;
            }

            this.currentRouteIndex = 0;
            var nextRoutePart = this.currentRoute[0];
            currentRoutePart.Remove(car);
            nextRoutePart.Add(car);
        }
    }
}
