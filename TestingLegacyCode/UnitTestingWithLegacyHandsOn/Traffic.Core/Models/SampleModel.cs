// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleModel.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the SampleModel
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models
{
    using System.Diagnostics;

    /// <summary>
    /// This class is used solely by the "Advanced Techniques" chapter, and does not interact with any other
    /// classes in the Traffic Simulator, or Hands on Lab.
    /// </summary>
    public class SampleModel
    {
        public SampleModel()
        {
            this.InstanceNumber = ++InstanceCount;
            Debug.WriteLine("Sample Model Constructor Invoked");
        }

        public static int InstanceCount { get; private set; }

        public int InstanceNumber { get; private set; }

        public void SampleMethod()
        {
            Debug.WriteLine("Sample Model SampleMethod Invoked");
        }
    }
}
