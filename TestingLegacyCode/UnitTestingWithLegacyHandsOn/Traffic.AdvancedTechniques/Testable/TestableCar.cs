// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestableCar.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the TestableCar
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable
{
    using System;
    using System.Collections.Generic;
    using System.Fakes;
    using System.Windows.Media;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Fakes;

    internal class TestableCar : TestableBase<ShimCar, Car>
    {
        private static readonly Dictionary<Car, RoutingAlgorithm> CarRoutes = new Dictionary<Car, RoutingAlgorithm>();
        private readonly Dictionary<Car, TestableCar> carShims = new Dictionary<Car, TestableCar>();

        public TestableCar(TestableTrafficEnvironment trafficEnvironment, Car car, RoutingAlgorithm algorithm, Brush arg3) : base(trafficEnvironment)
        {
            CarRoutes[car] = algorithm;
            var shim = new ShimCar(car)
            {
                RoutingSetRoutingAlgorithm = o => CarRoutes[car] = o,
                RoutingGet = delegate { return CarRoutes[car]; },
                ShouldMoveGet = delegate { return false; }
            };
            this.weakShim = new WeakReference<ShimCar>(shim);
            this.carShims.Add(car, this);
            shim.RandomGeneratorGet = () => new ShimRandom() { NextDouble = () => this.MoveIfAllowed ? 1.0 : 0.0 };
        }

        public bool MoveIfAllowed { get; set; }

        internal static void EnableShims(TestableTrafficEnvironment trafficEnvironment)
        {
            ShimCar.ConstructorRoutingAlgorithmBrush = delegate(Car car, RoutingAlgorithm algorithm, Brush arg3)
                                                           {
                                                               var shimCar = new TestableCar(trafficEnvironment, car, algorithm, arg3);
                                                           };
        }
    }
}