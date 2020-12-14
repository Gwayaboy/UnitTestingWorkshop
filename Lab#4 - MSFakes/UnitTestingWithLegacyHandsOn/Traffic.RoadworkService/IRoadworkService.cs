// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoadworkService.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the IRoadworkService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.RoadworkService
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRoadworkService" in both code and config file together.
    [ServiceContract]
    public interface IRoadworkService
    {
        [OperationContract]
        List<Impediment> RetrieveCurrent(List<Block> locations);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Impediment
    {
        [DataMember]
        private readonly Block location;
        [DataMember]
        private readonly string description;
        [DataMember]
        private readonly double relativeSpeed;

        internal Impediment(Block location, string description, double relativeSpeed)
        {
            this.location = location;
            this.description = description;
            this.relativeSpeed = relativeSpeed;
        }

        public Block Location
        {
            get { return this.location; }
        }

        public string Description
        {
            get { return this.description; }
        }

        public double RelativeSpeed
        {
            get { return this.relativeSpeed; }
        }
    }
}
