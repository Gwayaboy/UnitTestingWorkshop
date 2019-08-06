// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestableSampleModel.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the TestableCar
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable
{
    using System;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;

    internal class TestableSampleModel : TestableBase<ShimSampleModel, SampleModel>
    {
        public TestableSampleModel(TestableTrafficEnvironment trafficEnvironment, SampleModel model) : base(trafficEnvironment)
        {
            ++InstanceCount;
            var shim = new ShimSampleModel(model)
            {
            };
            this.weakShim = new WeakReference<ShimSampleModel>(shim);
            this.weakInstance = new WeakReference<SampleModel>(model);
        }

        public static int InstanceCount { get; private set; }

        public static void EnableShimsForClass()
        {
            ShimSampleModel.Constructor =
                (real) =>
                    {
                        ShimsContext.ExecuteWithoutShims(delegate
                        {
                            var constructor = typeof(SampleModel).GetConstructor(new Type[] { });
                            constructor.Invoke(real, new object[] { });
                        });
                        var testable = new TestableSampleModel(null, real);
                    };
        }

        public static implicit operator TestableSampleModel(SampleModel desired)
        {
            return (TestableSampleModel)Find(desired);
        }

        public static implicit operator TestableSampleModel(ShimSampleModel desired)
        {
            return (TestableSampleModel)Find(desired);
        }

        public static implicit operator SampleModel(TestableSampleModel desired)
        {
            return desired.Instance;
        }
    }
}