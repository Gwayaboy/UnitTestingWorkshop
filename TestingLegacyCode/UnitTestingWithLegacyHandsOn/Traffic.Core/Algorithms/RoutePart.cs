// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutePart.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the RoutePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    [DebuggerDisplay("{Road.Name}")]
    public class RoutePart
    {
        private const double Filter = 0.8;
        private readonly Dictionary<Car, DateTime> carTimes = new Dictionary<Car, DateTime>();
        private readonly double probability;
        private TimeSpan duration;

        public RoutePart(Block road)
        {
            this.Road = road;
            this.duration = TimeSpan.Zero;
            this.probability = 0.90;
        }

        public Block Road { get; set; }

        public double Probability
        {
            get
            {
                var result = this.probability;
                for (int i = 1; i <= this.carTimes.Count; ++i)
                {
                    result *= 0.95;
                }

                return this.probability;
            }
        }

        public TimeSpan Duration
        {
            get { return this.carTimes.Count == 0 ? TimeSpan.Zero : this.duration; }
        }

        public void Add(Car car)
        {
            car.Location.Road = this.Road;
            car.Location.Position = 0;
            this.Road.Add(car);
            this.carTimes[car] = DateTime.Now;
        }

        public void Remove(Car car)
        {
            this.Road.Remove(car);
            DateTime entry;
            if (this.carTimes.TryGetValue(car, out entry))
            {
                TimeSpan elapsed = DateTime.Now - entry;
                this.duration = TimeSpan.FromMilliseconds((this.duration.TotalMilliseconds * Filter) + (elapsed.TotalMilliseconds * (1 - Filter)));
                Trace.WriteLine(string.Format("{0} = {1}", this.Road.Name, this.Duration.TotalSeconds));
                this.carTimes.Remove(car);
            }
        }
    }
}