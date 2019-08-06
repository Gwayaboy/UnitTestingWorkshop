// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Route.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the Route type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    [DebuggerDisplay("{StartsAt.Name} / {EndsAt.Name} Length={Count} Duration={Duration}")]
    public class Route
    {
        private readonly List<RoutePart> parts;

        public Route()
        {
            this.parts = new List<RoutePart>();
            this.NumberOfTurns = 0;
        }

        public Route(RoutePart first)
        {
            this.parts = new List<RoutePart> { first };
            this.NumberOfTurns = 0;
            this.UpdateCount(first);
        }

        public Route(Route previous, RoutePart additional)
        {
            this.parts = new List<RoutePart>(previous.Parts);
            this.NumberOfTurns = previous.NumberOfTurns;
            this.NorthCount = previous.NorthCount;
            this.SouthCount = previous.SouthCount;
            this.EastCount = previous.EastCount;
            this.WestCount = previous.WestCount;
            this.UpdateCount(additional);
            if (this.IsTurn(additional))
            {
                ++this.NumberOfTurns;
            }

            this.parts.Add(additional); // Must be done after turn calculation!!!
        }

        public int NumberOfTurns { get; private set; }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan total = this.Parts.Aggregate(TimeSpan.Zero, (current, part) => current + part.Duration);
                return total;
            }
        }
        
        public int Count
        {
            get { return this.Parts.Count; }
        }

        public Intersection StartsAt
        {
            get { return this.Parts[0].Road.StartsAt; }
        }

        public Intersection EndsAt
        {
            get { return this.Parts[this.Parts.Count - 1].Road.EndsAt; }
        }

        public int NorthCount
        {
            get;
            private set;
        }

        public int SouthCount
        {
            get;
            private set;
        }

        public int EastCount
        {
            get;
            private set;
        }

        public int WestCount
        {
            get;
            private set;
        }

        public IReadOnlyList<RoutePart> Parts
        {
            get { return this.parts; }
        }

        public RoutePart this[int index]
        {
            get { return this.Parts[index]; }
        }

        public bool IsTurn(RoutePart additional)
        {
            if (this.Parts.Count == 0)
            {
                return false;
            }

            return this.Parts[this.Parts.Count - 1].Road.StreetDirection != additional.Road.StreetDirection;
        }

        public bool Contains(Block block)
        {
            return this.Parts.Any(part => part.Road == block);
        }

        public bool Contains(Intersection intersection)
        {
            return this.Parts.Any(part => part.Road.EndsAt == intersection);
        }

        public bool Contains(RoutePart item)
        {
            return this.Parts.Any(part => part == item);
        }
        
        private void UpdateCount(RoutePart part)
        {
            switch (part.Road.StreetDirection)
            {
                case Direction.North:
                    ++this.NorthCount;
                    break;
                case Direction.South:
                    ++this.SouthCount;
                    break;
                case Direction.East:
                    ++this.EastCount;
                    break;
                case Direction.West:
                    ++this.WestCount;
                    break;
            }
        }
    }
}