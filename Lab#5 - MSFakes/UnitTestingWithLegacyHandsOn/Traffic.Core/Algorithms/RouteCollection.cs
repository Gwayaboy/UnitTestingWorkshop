// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteCollection.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the RouteCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using System;
    using System.Collections.Generic;

    public class RouteCollection : IEnumerable<Route>
    {
        private readonly List<Route> routes = new List<Route>();
        private readonly List<Route> shortestRoutes = new List<Route>();

        public int Count
        {
            get { return this.routes.Count; }
        }

        public int ShortestDistance
        {
            get;
            private set;
        }

        public IReadOnlyList<Route> Routes
        {
            get { return this.routes; }
        }

        public IReadOnlyList<Route> ShortestRoutes
        {
            get { return this.shortestRoutes; }
        }

        public void Add(Route route)
        {
            if (route.Count == 0)
            {
                throw new Exception("Route has No Parts!");
            }

            int routeLength = route.Count;
            if (this.ShortestDistance == 0)
            {
                this.ShortestDistance = routeLength;
            }

            if (routeLength < this.ShortestDistance)
            {
                this.shortestRoutes.Clear();
                this.ShortestDistance = routeLength;
            }
            else if (routeLength == this.ShortestDistance)
            {
                this.shortestRoutes.Add(route);
            }

            this.routes.Add(route);
        }

        public IEnumerator<Route> GetEnumerator()
        {
            return this.routes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.routes.GetEnumerator();
        }
    }
}