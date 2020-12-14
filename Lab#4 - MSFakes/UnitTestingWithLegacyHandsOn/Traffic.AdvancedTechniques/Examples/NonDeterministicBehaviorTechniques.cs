// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonDeterministicBehaviorTechniques.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Summary description for NonDeterministicBehaviorTechniques
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Traffic.Core.AdvancedTechniques.Examples
{
    using System;
    using System.Threading;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// TimerIntercept_UnitTests
    /// </summary>
    [TestClass]
    public class NonDeterministicBehaviorTechniques
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return this.testContextInstance; }

            set { this.testContextInstance = value; }
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_InterceptTimer()
        {
            using (ShimsContext.Create())
            {
                // Eliminate The Service Calls as detailed in Exercise #1
                Helpers.ShimServiceCalls();

                // Now Get Control of the Timer
                TimerCallback applicationCallback = null;
                object state = null;
                TimeSpan interval = TimeSpan.Zero;
                System.Threading.Fakes.ShimTimer.ConstructorTimerCallbackObjectTimeSpanTimeSpan = (timer, callback, arg3, arg4, arg5) =>
                                                                                                      {
                                                                                                          applicationCallback = callback;
                                                                                                          state = arg3;
                                                                                                          interval = arg5;
                                                                                                      };

                // Start the Simulation
                City instance = new City { Run = true };

                // Verify internal time is NOT updating the simulation
                Thread.Sleep(TimeSpan.FromSeconds(10));
                Assert.AreEqual(0, instance.IterationCounter);

                // Do controlled invocations of iteration processing
                const int IterationCount = 10;
                for (int i = 1; i <= IterationCount; ++i)
                {
                    applicationCallback(state);
                    Thread.Sleep(interval);
                }

                // Verify that the simulation processed the number of expected iterations
                Assert.AreEqual(IterationCount, instance.IterationCounter);
            }
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_InterceptRandom()
        {
            using (ShimsContext.Create())
            {
                // Eliminate The Service Calls as detailed in Exercise #1
                Helpers.ShimServiceCalls();

                // Now Get Control of the Random Number Generator
                System.Fakes.ShimRandom.Constructor = (real) => { };
                System.Fakes.ShimRandom.AllInstances.NextDouble = this.NextDouble;
                System.Fakes.ShimRandom.AllInstances.NextInt32Int32 = this.NextInt32Int32;

                var instance = new City();
                instance.BuildCity();

                // Since the initialization logic does a Random(1,2) to determine North/West for initial placement
                // By forcing a return of 1 (see NextInt32Int32 method below) will ensure that they all end up West.
                instance.InitializeCars();

                var westBound = instance.Intersections[2, 2].Blocks[Direction.West];
                int carcount = Helpers.CollectCars(westBound).Count;

                // Random cars compromise 20% of the pool, so "Routed" cars will account for 80%
                double expected = City.NumberOfCars * 0.8;
                Assert.AreEqual(expected, carcount);
            }
        }

        private int NextInt32Int32(Random random, int i, int arg3)
        {
            return (i + arg3) / 2;
        }

        private double NextDouble(Random random)
        {
            return 0.5;
        }
    }
}