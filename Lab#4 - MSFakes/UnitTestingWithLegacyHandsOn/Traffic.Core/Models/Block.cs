// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Block.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the Block type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    [DebuggerDisplay("{Name}")]
    [DataContract]
    public class Block
    {
        private readonly List<Car> occupants = new List<Car>();
        private readonly object syncRoot = new object();
        private Intersection startsAt;
        private Intersection endsAt;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        [DataMember]
        public int LocationCount { get; set; }

        [DataMember]
        public Direction StreetDirection { get; set; }

        public IEnumerable<Car> Occupants
        {
            get
            {
                lock (this.syncRoot)
                {
                    return new List<Car>(this.occupants);
                }
            }
        }

        public Intersection StartsAt
        {
            get
            {
                return this.startsAt;
            }

            set
            {
                if ((value != null) && (this.endsAt == value))
                {
                    throw new Exception();
                }

                this.startsAt = value;
            }
        }

        public Intersection EndsAt
        {
            get
            {
                return this.endsAt;
            }

            set
            {
                if ((value != null) && (value == this.startsAt))
                {
                    throw new Exception();
                }

                this.endsAt = value;
            }
        }

        public bool IsFree(int position)
        {
            lock (this.syncRoot)
            {
                return this.occupants.All(occupant => occupant.Location.Position != position);
            }
        }

        public void Add(Car car)
        {
            if (car.Location.Road != this)
            {
                throw new Exception();
            }

            lock (this.syncRoot)
            {
                this.occupants.Add(car);
            }
        }

        public void Remove(Car car)
        {
            lock (this.syncRoot)
            {
                this.occupants.Remove(car);
            }
        }

        public bool HasExit()
        {
            return this.EndsAt.Blocks.Where(option => option.Value != null).Any(option => option.Value.StartsAt == this.EndsAt);
        }
    }
}
