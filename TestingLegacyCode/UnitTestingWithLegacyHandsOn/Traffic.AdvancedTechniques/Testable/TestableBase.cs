// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestableBase.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the TestableBase
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable
{
    using System;
    using System.Collections.Generic;

    internal class TestableBase<TShim, TInstance> where TShim : class where TInstance : class
    {
        protected WeakReference<TShim> weakShim;
        protected WeakReference<TInstance> weakInstance;
        private static readonly List<WeakReference<TestableBase<TShim, TInstance>>> Instances = new List<WeakReference<TestableBase<TShim, TInstance>>>();
        private readonly TestableTrafficEnvironment trafficEnvironment;

        protected TestableBase(TestableTrafficEnvironment trafficEnvironment)
        {
            this.trafficEnvironment = trafficEnvironment;
            Instances.Add(new WeakReference<TestableBase<TShim, TInstance>>(this));
        }

        public TShim Shim
        {
            get
            {
                TShim target;
                if (this.weakShim.TryGetTarget(out target))
                {
                    return target;
                }

                throw new Exception("Shim has been garbage collected");
            }
        }

        public TInstance Instance
        {
            get
            {
                TInstance target;
                if (this.weakInstance.TryGetTarget(out target))
                {
                    return target;
                }

                throw new Exception("Instance has been garbage collected");
            }
        }

        protected static TestableBase<TShim, TInstance> Find(TShim desired)
        {
            foreach (var instance in Instances)
            {
                TestableBase<TShim, TInstance> target;
                if (instance.TryGetTarget(out target))
                {
                    if (ReferenceEquals(target.Shim, desired))
                    {
                        return target;
                    }
                }
            }

            return null;
        }

        protected static TestableBase<TShim, TInstance> Find(TInstance desired)
        {
            foreach (var instance in Instances)
            {
                TestableBase<TShim, TInstance> target;
                if (instance.TryGetTarget(out target))
                {
                    if (ReferenceEquals(target.Instance, desired))
                    {
                        return target;
                    }
                }
            }

            return null;
        }
    }
}
