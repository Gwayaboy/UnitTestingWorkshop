// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Intersection.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the Intersection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Name}")]
    public class Intersection
    {
        private readonly Dictionary<Direction, Block> blocks = new Dictionary<Direction, Block>();

        public Intersection()
        {
            this.Blocks.Add(Direction.North, null);
            this.Blocks.Add(Direction.South, null);
            this.Blocks.Add(Direction.East, null);
            this.Blocks.Add(Direction.West, null);
        }

        public string Name { get; set; }

        public Dictionary<Direction, Block> Blocks
        {
            get { return this.blocks; }
        }
    }
}