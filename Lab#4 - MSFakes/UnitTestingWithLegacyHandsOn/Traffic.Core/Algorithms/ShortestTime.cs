// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortestTime.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the ShortestTime type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    public class ShortestTime : RoutingAlgorithm
    {
        protected override Route SelectBestRoute(Car car)
        {
            var options = DiscoveredRoutes.Find(StartTrip, EndTrip).Routes;
            var results = new List<Route>();
            var best = TimeSpan.MaxValue;
            foreach (var route in options)
            {
                TimeSpan total = route.Duration;
                if (total < best)
                {
                    results.Clear();
                    best = total;
                }

                if (total == best)
                {
                    results.Add(route);
                }
            }

            if ((options == null) || (options.Count == 0))
            {
                // No way to get there???
                return null;
            }

            var random = RoutingAlgorithm.RandomGenerator.Next(0, options.Count);
            return options[random];
        }
    }
}
