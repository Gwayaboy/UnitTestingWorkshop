// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Car.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the Car type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models
{
    using System;
    using System.Windows.Media;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms;

    public class Car
    {
        public Car(RoutingAlgorithm routing, Brush color)
        {
            this.VehicleColor = color;
            this.Routing = routing;

            // Random Version with Fixed Seed is used to cause Failure of Exercise #3 Test...
            // RandomGenerator = new Random(100);
            this.RandomGenerator = new Random();
        }

        public bool ShouldMove
        {
            get
            {
                if (this.Location == null)
                {
                    return false;
                }

                if (this.Location.Road == null)
                {
                    return false;
                }

                if (this.Location.Road.IsFree(this.Location.Position + 1))
                {
                    var routePart = DiscoveredRoutes.ToRoutePart(this.Location.Road);
                    if (routePart == null)
                    {
                        return false;
                    }

                    var probability = routePart.Probability;
                    return this.RandomGenerator.NextDouble() < probability;
                }

                return false;
            }
        }

        public Brush VehicleColor { get; set; }

        public RoutingAlgorithm Routing { get; set; }

        public ElementLocation Location { get; set; }

        public Random RandomGenerator { get; private set; }
    }
}
