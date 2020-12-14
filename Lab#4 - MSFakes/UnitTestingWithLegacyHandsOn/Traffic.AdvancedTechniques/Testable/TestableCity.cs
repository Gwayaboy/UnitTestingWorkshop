// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestableCity.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the TestableCity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Fakes;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal class TestableCity : TestableBase<ShimCity, City>
    {
        private static readonly List<TestableCity> Instances = new List<TestableCity>();
        private readonly City myCity;
        private TimerCallback applicationCallback;
        private object timerStateObject;
        private TimeSpan timerInterval;

        public TestableCity(TestableTrafficEnvironment trafficEnvironment)
            : base(trafficEnvironment)
        {
            this.myCity = new City();
            Instances.Add(this);
            if (trafficEnvironment.ShimsEnabled)
            {
                this.EnableShims();
            }
        }

        public static event EventHandler RunStateChanged;

        public void SingleStep()
        {
            Assert.IsNotNull(this.applicationCallback);
            this.applicationCallback(this.timerStateObject);
        }
        
        internal static void EnableShimsForAllInstances()
        {
            Helpers.ShimServiceCalls();
            foreach (var testable in Instances)
            {
                testable.EnableShims();
            }
        }

        private static void ReleaseTimerShim()
        {
            System.Threading.Fakes.ShimTimer.ConstructorTimerCallbackObjectTimeSpanTimeSpan = null;
        }

        private void EnableShims()
        {
            var shim = new ShimCity(this.myCity)
                       {
                           RunSetBoolean = this.RunShim,
                           InitializeRoutes = this.InitializeRoutesShim
                       };
            this.weakShim = new WeakReference<ShimCity>(shim);
         }

        private void ShimCityTimer()
        {
            this.applicationCallback = null;
            this.timerStateObject = null;
            this.timerInterval = TimeSpan.Zero;
            System.Threading.Fakes.ShimTimer.ConstructorTimerCallbackObjectTimeSpanTimeSpan = (timer, callback, arg3, arg4, arg5) =>
                                                                                                  {
                                                                                                      applicationCallback = callback;
                                                                                                      timerStateObject = arg3;
                                                                                                      timerInterval = arg5;
                                                                                                  };
        }

        private void RunShim(bool arg1)
        {
            Shim.RunSetBoolean = null;
            if (arg1)
            {
                this.ShimCityTimer();
            }

            this.Shim.Instance.Run = arg1;
            if (arg1)
            {
                ReleaseTimerShim();
            }

            var shadow = RunStateChanged;
            this.Shim.RunSetBoolean = this.RunShim;

            if (shadow != null)
            {
                shadow(this, new EventArgs());
            }
        }

        private void InitializeRoutesShim()
        {
            var forEachShim =
                new FakesDelegates.Func<IEnumerable<Tuple<Intersection, Intersection, Route>>, System.Action<Tuple<Intersection, Intersection, Route>>, ParallelLoopResult>(
                    delegate(IEnumerable<Tuple<Intersection, Intersection, Route>> tuples, Action<Tuple<Intersection, Intersection, Route>> arg3)
                    {
                        if (tuples != null)
                        {
                            foreach (var tuple in tuples)
                            {
                                if (tuple.Item1 == this.Shim.Instance.Intersections[2, 2])
                                {
                                    arg3(tuple);
                                }
                            }
                        }

                        return new ParallelLoopResult();
                    });

            ShimParallel.ForEachOf1IEnumerableOfM0ActionOfM0(forEachShim);
            this.Shim.InitializeRoutes = null;
            this.Shim.Instance.InitializeRoutes();
            this.Shim.InitializeRoutes = this.InitializeRoutesShim;
        }
    }
}