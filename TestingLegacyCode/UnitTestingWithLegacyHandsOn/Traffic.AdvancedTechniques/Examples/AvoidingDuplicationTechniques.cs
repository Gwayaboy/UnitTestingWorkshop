// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvoidingDuplicationTechniques.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Summary description for AvoidingDuplicationTechniques
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Traffic.Core.AdvancedTechniques.Examples
{
    using System;
    using System.Threading;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// TimerIntercept_UnitTests
    /// </summary>
    [TestClass]
    public class AvoidingDuplicationTechniques
    {
        private static TestableTrafficEnvironment testableTrafficEnvironment;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        /// <summary>
        ///  Use ClassInitialize to run code before running the first test in the class
        /// </summary>
        /// <param name="testContext">TestContext</param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            Console.WriteLine("MyClassInitialize");
            Assert.IsNull(testableTrafficEnvironment, "ShimsContext set before Class Initialized");
            testableTrafficEnvironment = new TestableTrafficEnvironment();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Console.WriteLine("MyClassCleanup");
            Assert.IsNotNull(testableTrafficEnvironment, "ShimsContext NOT set before Class Cleanup");
            testableTrafficEnvironment.Dispose();
            testableTrafficEnvironment = null;
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test 
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            Console.WriteLine("MyTestInitialize");
            Assert.IsNotNull(testableTrafficEnvironment, "ShimsContext NOT set before Test Initialize");
            testableTrafficEnvironment.EnableShims();
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup]
        public void MyTestCleanup()
        {
            Console.WriteLine("MyTestCleanup");
            Assert.IsNotNull(testableTrafficEnvironment, "ShimsContext NOT set before Test Cleanup");
            testableTrafficEnvironment.DisableShims();
        }

        #endregion

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_Doppelgängers()
        {
            using (var shimContext = ShimsContext.Create())
            {
                TestableSampleModel.EnableShimsForClass();
                
                // Make sure insance creation occurs...
                int initialCounterValue = SampleModel.InstanceCount;
                int initialTestableCounterValue = TestableSampleModel.InstanceCount;
                var instance = new SampleModel();
                Assert.IsNotNull(instance);
                Assert.IsInstanceOfType(instance, typeof(SampleModel));
                Assert.AreEqual(initialCounterValue + 1, SampleModel.InstanceCount);
                Assert.AreEqual(initialTestableCounterValue + 1, TestableSampleModel.InstanceCount);

                // Verify we can get the testable (implicitly) from instance
                TestableSampleModel testable = instance;
                Assert.IsNotNull(testable);
                Assert.IsInstanceOfType(testable, typeof(TestableSampleModel));

                // Verify we can get back to the instance (implicitly)
                SampleModel back = testable;
                Assert.IsNotNull(back);
                Assert.IsInstanceOfType(back, typeof(SampleModel));
                Assert.AreSame(instance, back);

                // Verify we can get shim from testable (via property)
                ShimSampleModel shim = testable.Shim;
                Assert.IsNotNull(shim);
                Assert.IsInstanceOfType(shim, typeof(ShimSampleModel));

                // Verify we can get the testable (implicitly) from shim
                TestableSampleModel testable2 = shim;
                Assert.IsNotNull(testable2);
                Assert.IsInstanceOfType(testable2, typeof(TestableSampleModel));
                Assert.AreSame(testable, testable2);
            }
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_BasicEnvironment()
        {
            int runStateChanged = 0;
            TestableCity.RunStateChanged += (sender, args) => Interlocked.Increment(ref runStateChanged);
            testableTrafficEnvironment.City.Run = true;
            Assert.AreEqual(1, runStateChanged, "Incorrect Number of Transitions after Run=true");
            Assert.IsTrue(testableTrafficEnvironment.City.Run, "Failed to Mark instance as Running");
            Assert.AreEqual(0, testableTrafficEnvironment.City.IterationCounter);

            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.AreEqual(1, runStateChanged, "Unexpected State Change while Running");
            Assert.IsTrue(testableTrafficEnvironment.City.Run, "City Unexpectedly stopped Running");
            Assert.AreEqual(0, testableTrafficEnvironment.City.IterationCounter, "Testable Instance is Running Timer");
            testableTrafficEnvironment.City.Run = false;
            Assert.AreEqual(2, runStateChanged, "Incorrect Number of Transitions after Run=false");
            Assert.IsFalse(testableTrafficEnvironment.City.Run, "Failed to Mark instance as Stopped");
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_InitializeRoutes()
        {
            testableTrafficEnvironment.City.InitializeRoutes();
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_SingleStep()
        {
            int initialTest = testableTrafficEnvironment.City.IterationCounter;
            testableTrafficEnvironment.TestableCityInstance.SingleStep();
            Thread.Sleep(TimeSpan.FromSeconds(1)); // To give timer time, in cases it is running, and should fail test.
            Assert.AreEqual(initialTest + 1, testableTrafficEnvironment.City.IterationCounter, "Single Step did not Execute Exactly One Iteration");
        }
    }
}
