// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGatheringTechniques.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Summary description for DataGatheringTechniques
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Traffic.Core.AdvancedTechniques.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms.Fakes;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// TimerIntercept_UnitTests
    /// </summary>
    [TestClass]
    public class DataGatheringTechniques
    {
        private readonly List<int> values = new List<int>();
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        private bool IsArmed { get; set; }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_ShowRandomSeedIssue()
        {
            Random random1 = new Random(100);
            Random random2 = new Random(100);
            double value1 = random1.NextDouble();
            double value2 = random2.NextDouble();
            Assert.AreEqual(value1, value2);
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_VerifyUniqueRandomSeeds()
        {
            bool passed = true;
            using (ShimsContext.Create())
            {
                // Eliminate The Service Calls as detailed in Exercise #1
                Helpers.ShimServiceCalls();

                // Now Get Control of the Random Number Generator
                // TODO : This accomplishes the goal, note that the actual instance must be initialized via reflection
                // TODO : Athough Ramdom can be Stubbed, that approach would not work for sealed classes (such as used later)
                // TODO : We need to also shim the time based default constructor so that it does not hit our "checking" constructor
                System.Fakes.ShimRandom.Constructor = delegate(Random random)
                {
                    ShimsContext.ExecuteWithoutShims(delegate
                    {
                        var constructor = typeof(Random).GetConstructor(new Type[] { });
                        constructor.Invoke(random, new object[] { });
                    });
                };
                System.Fakes.ShimRandom.ConstructorInt32 = delegate(Random random, int i)
                {
                    ShimsContext.ExecuteWithoutShims(delegate
                    {
                        var constructor = typeof(Random).GetConstructor(new[] { typeof(int) });
                        constructor.Invoke(random, new object[] { i });
                    });
                    if (this.values.Contains(i))
                    {
                        passed = false;
                        Assert.Fail("Multiple Random instances Created with identical seed Value={0}", i);
                    }

                    this.values.Add(i);
                };

                this.values.Clear();
                var instance = new City();
                instance.BuildCity();
                instance.InitializeCars();
                instance.Run = true;
                Thread.Sleep(TimeSpan.FromSeconds(5));
                instance.Run = false;
                Assert.IsTrue(passed, "Failure Occurred on Delegated Thread");
            }
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_VerifyPotentialRoutes()
        {
            using (ShimsContext.Create())
            {
                // Eliminate The Service Calls as detailed in Exercise #1
                Helpers.ShimServiceCalls();
                List<Route> consideredRoutes = new List<Route>();
                MethodInfo mi = typeof(ShortestTime).GetMethod("SelectBestRoute", BindingFlags.Instance | BindingFlags.NonPublic);
                System.Collections.Generic.Fakes.ShimList<Route>.AllInstances.AddT0 =
                    (collection, route) =>
                    ShimsContext.ExecuteWithoutShims(() =>
                    {
                        if (this.IsArmed)
                        {
                            consideredRoutes.Add(route);
                        }

                        collection.Add(route);
                    });

                // TODO: We can Shim the protected method, but without using reflection, there is no way to invoke it from within the shim
                // FYI: ExecuteWithoutShims disables ALL Shims, thereby breaking the capture of "consideredRoutes", but setting the individual shim to null works.
                FakesDelegates.Func<ShortestTime, Car, Route> shim = null;

                shim = (time, car) =>
                {
                    Route route = null;
                    IsArmed = true;
                    ShimShortestTime.AllInstances.SelectBestRouteCar = null;
                    var result = mi.Invoke(time, new object[] { car });
                    ShimShortestTime.AllInstances.SelectBestRouteCar = shim;
                    route = (Route)result;
                    IsArmed = false;
                    Assert.IsTrue(consideredRoutes.Count > 0, string.Format("Failed to Find Any Considered Routes from {0} to {1}", car.Routing.StartTrip.Name, car.Routing.EndTrip.Name));
                    return route;
                };
                ShimShortestTime.AllInstances.SelectBestRouteCar = shim;
                var instance = new City();
                instance.BuildCity();
                instance.InitializeRoutes();
                instance.InitializeCars();
            }
        }
    }
}