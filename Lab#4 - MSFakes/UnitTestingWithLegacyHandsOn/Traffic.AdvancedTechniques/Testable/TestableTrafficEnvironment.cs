// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestableTrafficEnvironment.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the TestableTrafficEnvironment
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable
{
    using System;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.QualityTools.Testing.Fakes;

    internal class TestableTrafficEnvironment : IDisposable
    {
        private readonly TestableCity testableCity;
        private IDisposable shimsContext;

        public TestableTrafficEnvironment()
        {
            this.EnableShims();
            this.testableCity = new TestableCity(this);
            this.DisableShims();
        }

        public bool ShimsEnabled { get; private set; }

        public TestableCity TestableCityInstance
        {
            get { return this.testableCity; }
        }

        public City City
        {
            get { return this.testableCity.Shim; }
        }

        public void EnableShims()
        {
            this.ShimsEnabled = true;
            this.shimsContext = ShimsContext.Create();
            TestableCar.EnableShims(this);
            TestableCity.EnableShimsForAllInstances();
        }

        public void Dispose()
        {
            if (this.shimsContext != null)
            {
                this.shimsContext.Dispose();
                this.shimsContext = null;
            }
        }

        internal void DisableShims()
        {
            this.ShimsEnabled = false;
            this.shimsContext.Dispose();
            this.shimsContext = null;
        }
    }
}
