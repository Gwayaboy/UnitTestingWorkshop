// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceImplementation.svc.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the RoadworkService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.RoadworkService
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    public class RoadworkService : IRoadworkService
    {
        #region IRoadworkService Members

        public List<Impediment> RetrieveCurrent(List<Block> locations)
        {
            return locations.Select(location => new Impediment(location, "Dummy", 0.5)).ToList();
        }

        #endregion
    }
}