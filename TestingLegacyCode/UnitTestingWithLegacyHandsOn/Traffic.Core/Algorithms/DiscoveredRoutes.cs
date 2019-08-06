// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscoveredRoutes.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the DiscoveredRoutes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    internal static class DiscoveredRoutes
    {
        private static readonly Dictionary<Intersection, Dictionary<Intersection, RouteCollection>> PossibleRoutes = new Dictionary<Intersection, Dictionary<Intersection, RouteCollection>>();
        private static Dictionary<Block, RoutePart> singleRouteParts;

        private static Dictionary<Intersection, RouteCollection> blockMap;

        private static List<Tuple<Intersection, Intersection, Route>> existingRoutes;

        public static RoutePart ToRoutePart(Block block)
        {
            return singleRouteParts[block];
        }

        internal static void BuildAll(City city, int maxTurns)
        {
            bool more = true;
            while (more)
            {
                int discardedBlocks = 0;
                int discardedIntersections = 0;
                int tooManyTurns = 0;
                int zigZag = 0;
                int length = 0;
                var newRoutes = new ConcurrentBag<Tuple<Intersection, Intersection, Route>>();
                Parallel.ForEach(
                    existingRoutes,
                    existingRoute =>
                        {
                            RouteCollection newBlockPart = blockMap[existingRoute.Item2];
                            if (newBlockPart.Count > 2)
                            {
                                throw new Exception("More than Two exits!!!!");
                            }

                            foreach (var extension in newBlockPart)
                            {
                                if (extension.Parts.Count != 1)
                                {
                                    throw new Exception("Not A Single Block Extension");
                                }

                                if (existingRoute.Item3.Contains(extension.Parts[0]))
                                {
                                    // Already Traveled this Block...
                                    Interlocked.Increment(ref discardedBlocks);
                                }
                                else if (existingRoute.Item3.Contains(extension.EndsAt))
                                {
                                    // We have Already Been to That Intersection!
                                    Interlocked.Increment(ref discardedIntersections);
                                }
                                else if (existingRoute.Item3.Parts[0].Road.StartsAt == extension.EndsAt)
                                {
                                    // This would take us back to where we started!!!
                                    Interlocked.Increment(ref discardedIntersections);
                                }
                                else if (existingRoute.Item3.IsTurn(extension.Parts[0]) && (existingRoute.Item3.NumberOfTurns >= maxTurns))
                                {
                                    // We are Turning Too Much
                                    Interlocked.Increment(ref tooManyTurns);
                                }
                                else if ((existingRoute.Item3.NorthCount >= 2 && existingRoute.Item3.SouthCount >= 2) ||
                                         (existingRoute.Item3.EastCount >= 2 && existingRoute.Item3.WestCount >= 2))
                                {
                                    // Too much back and forth...
                                    Interlocked.Increment(ref zigZag);
                                }
                                else
                                {
                                    var newRoute = new Route(existingRoute.Item3, extension.Parts[0]);
                                    newRoutes.Add(new Tuple<Intersection, Intersection, Route>(existingRoute.Item1, extension.EndsAt, newRoute));
                                    length = newRoute.Count;
                                }
                            }
                        });
                foreach (var newRoute in newRoutes)
                {
                    Add(newRoute.Item1, newRoute.Item2, newRoute.Item3);
                }

                more = newRoutes.Count > 0;
                int totalCount = PossibleRoutes.Values.SelectMany(a => a.Values).Sum(b => b.Count);
                using (var writer = new StreamWriter("BuildCity.log", true))
                {
                    writer.WriteLine("\tFound {0} new Routes of length: {1}  - Total={2}, Pruned={3}/{4}/{5}/{6}", newRoutes.Count, length, totalCount, discardedBlocks, discardedIntersections, tooManyTurns, zigZag);
                }

                existingRoutes = new List<Tuple<Intersection, Intersection, Route>>(newRoutes);
            }
        }

        internal static void InitializeBaseline(City city)
        {
            PossibleRoutes.Clear();
            singleRouteParts = new Dictionary<Block, RoutePart>();
            blockMap = new Dictionary<Intersection, RouteCollection>();
            existingRoutes = new List<Tuple<Intersection, Intersection, Route>>();
            CreateSingleBlockRoutes(city, singleRouteParts, existingRoutes, blockMap);
        }
        
        internal static void Add(Intersection start, Intersection end, Route route)
        {
            // Trace.WriteLine(String.Format("Recording from {0} to {1} in {2} segments", start.Name, end.Name, route.Count));
            if (route.Count == 0)
            {
                throw new Exception("Empty Route");
            }

            if (route[0].Road.StartsAt != start)
            {
                throw new Exception("Invalid Start");
            }

            if (route[route.Count - 1].Road.EndsAt != end)
            {
                throw new Exception("Invalid End");
            }

            Dictionary<Intersection, RouteCollection> entry;
            if (!PossibleRoutes.TryGetValue(start, out entry))
            {
                entry = new Dictionary<Intersection, RouteCollection>();
                PossibleRoutes.Add(start, entry);
            }

            RouteCollection routes;
            if (!entry.TryGetValue(end, out routes))
            {
                routes = new RouteCollection();
                entry.Add(end, routes);
            }

            if (routes.Contains(route))
            {
                throw new Exception("Attempt to add Duplicate Route");
            }

            routes.Add(route);
        }

        internal static RouteCollection Find(Intersection start, Intersection end)
        {
            Dictionary<Intersection, RouteCollection> entry;
            if (!PossibleRoutes.TryGetValue(start, out entry))
            {
                return new RouteCollection();
            }

            RouteCollection routes;
            if (!entry.TryGetValue(end, out routes))
            {
                return new RouteCollection();
            }

            // Trace.WriteLine(String.Format("Found from {0} to {1} with {2} routes", start.Name, end.Name, routes.Count));
            return routes;
        }

        private static void CreateSingleBlockRoutes(City city, Dictionary<Block, RoutePart> allBlocks, List<Tuple<Intersection, Intersection, Route>> existingRoutes, Dictionary<Intersection, RouteCollection> blockMap)
        {
            for (int street = 1; street <= City.NumStreets; ++street)
            {
                for (int avenue = 1; avenue <= City.NumAvenues; ++avenue)
                {
                    var intersection = city.Intersections[street, avenue];
                    RouteCollection routes = new RouteCollection();
                    blockMap.Add(intersection, routes);
                    if (intersection != null)
                    {
                        foreach (var block in intersection.Blocks.Values)
                        {
                            if ((block != null) && (block.StartsAt == intersection))
                            {
                                var routePart = new RoutePart(block);
                                allBlocks.Add(block, routePart);
                                var route = new Route(routePart);
                                existingRoutes.Add(new Tuple<Intersection, Intersection, Route>(block.StartsAt, block.EndsAt, route));
                                Add(block.StartsAt, block.EndsAt, route);
                                routes.Add(route);
                            }
                        }
                    }
                }
            }
        }
    }
}
