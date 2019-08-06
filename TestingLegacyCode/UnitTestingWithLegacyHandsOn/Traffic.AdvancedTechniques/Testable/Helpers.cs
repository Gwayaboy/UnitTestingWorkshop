// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the Helpers type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.AdvancedTechniques.Testable
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.RoadworkService;
    using Microsoft.QualityTools.Testing.Fakes;
    using Block = Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Block;
    using Direction = Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models.Direction;
    using Impediment = Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.RoadworkServiceReference.Impediment;

    internal class Helpers
    {
        private static readonly Dictionary<RoadworkServiceClient, RoadworkService> Services = new Dictionary<RoadworkServiceClient, RoadworkService>();
        private static TimerCallback applicationCallback;
        private static object timerState;

        internal static void ShimServiceCalls()
        {
            Traffic.Core.RoadworkServiceReference.Fakes.ShimRoadworkServiceClient.Constructor =
                (real) =>
                {
                    Services.Add(real, new Traffic.RoadworkService.RoadworkService());
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
                        Traffic.Core.Models.Block[] paramTransformed = DataContractTransform<Traffic.Core.RoadworkServiceReference.Block[], Traffic.Core.Models.Block[]>(blocks);
                        var realBlocks = new List<Traffic.Core.Models.Block>(paramTransformed);
                        var service = Services[instance];
                        var results = service.RetrieveCurrent(realBlocks);
                        Traffic.Core.RoadworkServiceReference.Impediment[] resultTransformed = DataContractTransform<List<Traffic.RoadworkService.Impediment>, Traffic.Core.RoadworkServiceReference.Impediment[]>(results);
                        return resultTransformed;
                    });
            Traffic.Core.RoadworkServiceReference.Fakes.ShimRoadworkServiceClient.AllInstances.RetrieveCurrentBlockArray = intercept;
        }

        internal static void FireTimer()
        {
            applicationCallback(timerState);
        }

        internal static void ShimTimerCallbacks()
        {
            // Now Get Control of the Timer
            applicationCallback = null;
            timerState = null;
            TimeSpan interval = TimeSpan.Zero;
            System.Threading.Fakes.ShimTimer.ConstructorTimerCallbackObjectTimeSpanTimeSpan = (timer, callback, arg3, arg4, arg5) =>
                                                                                                  {
                                                                                                      applicationCallback = callback;
                                                                                                      timerState = arg3;
                                                                                                      interval = arg5;
                                                                                                  };
        }

        internal static TOutput DataContractTransform<TInput, TOutput>(TInput src)
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

        internal static List<Car> CollectCars(City city)
        {
            var retVal = new List<Car>();
            for (int street = 1; street < City.NumStreets; ++street)
            {
                for (int avenue = 1; avenue < City.NumAvenues; ++avenue)
                {
                    {
                        var model = city.Intersections[street, avenue].Blocks[Direction.West];
                        retVal.AddRange(new List<Car>(model.Occupants));
                    }

                    {
                        var model = city.Intersections[street, avenue].Blocks[Direction.North];
                        retVal.AddRange(new List<Car>(model.Occupants));
                    }
                }
            }

            return retVal;
        }

        internal static List<Car> CollectCars(Traffic.Core.Models.Block block)
        {
            return block.Occupants.ToList();
        }

        internal static List<Traffic.Core.Models.Block> CollectBlocks(City city)
        {
            var retVal = new List<Traffic.Core.Models.Block>();
            for (int street = 1; street < City.NumStreets; ++street)
            {
                for (int avenue = 1; avenue < City.NumAvenues; ++avenue)
                {
                    var intersection = city.Intersections[street, avenue];
                    foreach (var block in intersection.Blocks.Values)
                    {
                        if ((block != null) && (!retVal.Contains(block)))
                        {
                            retVal.Add(block);
                        }
                    }
                }
            }

            return retVal;
        }
    }

    internal class ObservableShimsContext : IDisposable
    {
        private readonly IDisposable context;

        public ObservableShimsContext()
        {
            this.context = ShimsContext.Create();
        }

        public event EventHandler OnDisposed;
        
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (this.Disposed)
            {
                return;
            }

            var shadow = this.OnDisposed;
            if (shadow != null)
            {
                shadow(this, new EventArgs());
            }

            this.Disposed = true;
            this.context.Dispose();
        }
    }
}
