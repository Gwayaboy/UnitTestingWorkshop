// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BreakingServiceBoundaryTechniques.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Summary description for BreakingServiceBoundaryTechniques
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Traffic.Core.AdvancedTechniques.Examples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.RoadworkService;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Impediment = Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Impediment;

    /// <summary>
    /// ServiceBoundary_UnitTests
    /// </summary>
    [TestClass]
    public class BreakingServiceBoundaryTechniques
    {
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

        /// <summary>
        /// This test is included to demonstrate that an attempt to call the service will fail. It is 
        /// marked with [Ignore] to prevent Test runs from failing..
        /// </summary>
        [Ignore]
        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_MissingService()
        {
            var city = new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.City();
            city.Run = true;
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Assert.IsTrue(city.Run);
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_InterceptClient()
        {
            using (ShimsContext.Create())
            {
                Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Fakes.ShimRoadworkServiceClient.Constructor =
                    (real) =>
                    {
                    };

                // var fakeClient = new RoadworkServiceReference.Fakes.ShimRoadworkServiceClient();
                var intercept = new FakesDelegates.Func<RoadworkServiceClient, Block[], Impediment[]>(
                    (instance, blocks) =>
                    {
                        var impediments = new List<Impediment>();
                        foreach (var item in blocks)
                        {
                            impediments.Add(new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Impediment() { description = "Testing", location = item, relativeSpeed = 0.5 });
                        }

                        return impediments.ToArray();
                    });
                Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Fakes.ShimRoadworkServiceClient.AllInstances.RetrieveCurrentBlockArray = intercept;

                var city = new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.City();
                city.Run = true;
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Assert.IsTrue(city.Run);
            }
        }

        [TestMethod]
        [TestCategory("AdvancedTechniques")]
        public void TechniqueSample_InterceptClient_DirectCall()
        {
            var services = new Dictionary<RoadworkServiceClient, RoadworkService>();
            using (ShimsContext.Create())
            {
                Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Fakes.ShimRoadworkServiceClient.Constructor = real =>
                                                                                           {
                                                                                               services.Add(real, new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.RoadworkService.RoadworkService());
                                                                                           };

                // var fakeClient = new RoadworkServiceReference.Fakes.ShimRoadworkServiceClient();
                var intercept = new FakesDelegates.Func<RoadworkServiceClient, Block[], Impediment[]>(
                    (instance, blocks) =>
                        {
                            // =====================================================================================
                            // The following (commented out) code uses explicit transforms, see documentation for
                            // reasons this may rapidly become difficult, and other potential issues..
                            // =====================================================================================

                            // var realBlocks = new List<Models.Block>();
                            // foreach (RoadworkServiceReference.Block item in blocks)
                            // {
                            //    var realBlock = Transform(item);
                            //    realBlocks.Add(realBlock);
                            // }
                            Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Block[] dataContractTransform = DataContractTransform<Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Block[], Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Block[]>(blocks);
                            var realBlocks = new List<Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Block>(dataContractTransform);
                            var service = services[instance];
                            var results = service.RetrieveCurrent(realBlocks);
                            var impediments = new List<Impediment>();
                            foreach (var result in results)
                            {
                                var clientImpediment = new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Impediment();
                                clientImpediment.location = Transform(result.Location);
                                impediments.Add(clientImpediment);
                            }

                            return impediments.ToArray();
                        });
                Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Fakes.ShimRoadworkServiceClient.AllInstances.RetrieveCurrentBlockArray = intercept;

                var city = new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.City { Run = true };
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Assert.IsTrue(city.Run);
            }
        }

        private static TOutput DataContractTransform<TInput, TOutput>(TInput src)
        {
            var serializer = new DataContractSerializer(typeof(TInput));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, src);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var deserializer = new DataContractSerializer(typeof(TOutput));
            var result = deserializer.ReadObject(stream);
            return (TOutput)result;
        }

        private static Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Block Transform(Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Block item)
        {
            var realBlock = new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Block
                                {
                                    Name = item.Name, 
                                    ShortName = item.ShortName, 
                                    //// EndsAt = Transform(item.EndsAt), 
                                    //// StartsAt = Transform(item.StartsAt)
                                };
            return realBlock;
        }

        private static Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Block Transform(Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Block item)
        {
            var realBlock = new Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Block
                                {
                                    Name = item.Name, 
                                    ShortName = item.ShortName, 
                                    //// EndsAt = Transform(item.EndsAt), 
                                    //// StartsAt = Transform(item.StartsAt)
                                };
            return realBlock;
        }

        ////private static Models.Intersection Transform(RoadworkServiceReference.Intersection item)
        ////{
        ////    var realIntersection = new Models.Intersection
        ////                               {
        ////                                   Name = item.Name
        ////                               };
        ////    return realIntersection;
        ////}

        ////private static RoadworkServiceReference.Intersection Transform(Models.Intersection item)
        ////{
        ////    var realIntersection = new RoadworkServiceReference.Intersection
        ////                               {
        ////                                   Name = item.Name
        ////                               };
        ////    return realIntersection;
        ////}
    }
}
