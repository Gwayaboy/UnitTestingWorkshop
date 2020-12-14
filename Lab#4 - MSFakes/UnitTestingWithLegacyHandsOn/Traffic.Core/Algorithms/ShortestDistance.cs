// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortestDistance.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the ShortestDistance type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    public class ShortestDistance : RoutingAlgorithm
    {
        protected override Route SelectBestRoute(Car car)
        {
            var options = DiscoveredRoutes.Find(StartTrip, EndTrip).ShortestRoutes;
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
