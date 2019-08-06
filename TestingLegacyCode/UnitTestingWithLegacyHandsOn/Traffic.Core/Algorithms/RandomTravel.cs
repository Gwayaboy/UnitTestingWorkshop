// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RandomTravel.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the RandomTravel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    public class RandomTravel : RoutingAlgorithm
    {
        private readonly Random random = new Random();

        protected override Route SelectBestRoute(Car car)
        {
            var intersection = car.Location.Road.EndsAt;
            if (intersection != null)
            {
                List<Block> options = new List<Block>();
                foreach (var option in intersection.Blocks)
                {
                    if (option.Value != null)
                    {
                        if ((option.Value.StartsAt == intersection) && option.Value.HasExit())
                        {
                            Trace.WriteLine(string.Format("\t Option: {0} on {1}", option.Value.StreetDirection, option.Value.Name));
                            options.Add(option.Value);
                        }
                    }
                }

                if (options.Count > 0)
                {
                    int index = this.random.Next(0, options.Count);
                    Block block = options[index];
                    var routePart = DiscoveredRoutes.ToRoutePart(block);
                    var route = new Route(routePart);
                    return route;
                }
            }

            return null;
        }
    }
}
