// --------------------------------------------------------------------------------------------------------------------
// <copyright file="City.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the City
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows.Media;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Algorithms;

    public class City
    {
        public const int NumStreets = 15;
        public const int NumAvenues = 10;
        public const int NumberOfCars = 500;
        internal List<Car> Cars = new List<Car>();
        private readonly List<Block> blocks = new List<Block>();
        private readonly Intersection[,] intersections = new Intersection[NumStreets + 1, NumAvenues + 1];
        private bool run;
        private int interlock1;
        private int phase;
        private Timer timer;
        
        public bool Run
        {
            get
            {
                return this.run;
            }

            set
            {
                this.run = value;
                if (this.run)
                {
                    this.IterationCounter = 0;
                    this.timer = new Timer(this.OnTimer, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
                }
                else
                {
                    if (this.timer != null)
                    {
                        var tmp = this.timer;
                        this.timer = null;

                        // Not sure why Dispose occasionally throws a NullReferenceException
                        // but we can ignore it.
                        try
                        {
                            var timerDisposed = new AutoResetEvent(false);
                            tmp.Dispose(timerDisposed);
                            timerDisposed.WaitOne();
                        }
                        catch (NullReferenceException)
                        {
                        }
                    }

                    this.IterationCounter = -1;
                }
            }
        }

        public Intersection[,] Intersections
        {
            get { return this.intersections; }
        }

        public int IterationCounter { get; private set; }

        private Intersection StartTrip
        {
            get { return this.Intersections[2, 2]; }
        }

        private Intersection EndTrip
        {
            get { return this.Intersections[NumStreets - 1, NumAvenues - 1]; }
        }
        
        public void BuildCity()
        {
            this.PopulateIntersections();
            for (int street = 1; street <= NumStreets; ++street)
            {
                if ((street % 2) == 0)
                {
                    this.BuildWestBoundStreet(street);
                }
                else
                {
                    this.BuildEastBoundStreet(street);
                }
            }

            for (int avenue = 1; avenue <= NumAvenues; ++avenue)
            {
                if ((avenue % 2) == 0)
                {
                    this.BuildUptownAvenue(avenue);
                }
                else
                {
                    this.BuildDowntownAvenue(avenue);
                }
            }

            DiscoveredRoutes.InitializeBaseline(this);
        }

        public void InitializeRoutes()
        {
            int maxTurns = Math.Max(NumAvenues, NumStreets) + 2;
            var sw = new Stopwatch();
            sw.Start();
            DiscoveredRoutes.BuildAll(this, maxTurns);
            var elapsed = sw.Elapsed;
            using (var writer = new StreamWriter("BuildCity.log", true))
            {
                writer.WriteLine("BuildAll() - {0} seconds to Build City with maximum {1} turns", elapsed.TotalSeconds, maxTurns);
            }
        }
        
        public void InitializeCars()
        {
            var r = new Random((int)DateTime.Now.Ticks);
            for (int i = 1; i <= NumberOfCars; ++i)
            {
                switch (i % 5)
                {
                    case 0:
                    case 1:
                        this.AddShortestDistanceCar(r);
                        break;
                    case 2:
                    case 3:
                        this.AddShortestTimeCar(r);
                        break;
                    case 4:
                        this.AddRandomRouteCar(r);
                        break;
                }
            }
        }

        private void BuildWestBoundStreet(int street)
        {
            for (int avenue = 1; avenue < NumAvenues; ++avenue)
            {
                var block = new Block
                {
                    LocationCount = 10,
                    StreetDirection = Direction.West,
                    Name = string.Format("{0} street between {1} and {2} avenue", street, avenue, avenue + 1),
                    ShortName = string.Format("{0} St  {1}/{2} Ave", street, avenue, avenue + 1),
                    StartsAt = this.intersections[street, avenue],
                    EndsAt = this.intersections[street, avenue + 1]
                };

                this.intersections[street, avenue].Blocks[Direction.West] = block;
                this.intersections[street, avenue + 1].Blocks[Direction.East] = block;
                this.blocks.Add(block);
            }
        }

        private void BuildEastBoundStreet(int street)
        {
            for (int avenue = 2; avenue <= NumAvenues; ++avenue)
            {
                var block = new Block
                {
                    LocationCount = 10,
                    StreetDirection = Direction.East,
                    Name = string.Format("{0} street between {1} and {2} avenue", street, avenue, avenue - 1),
                    ShortName = string.Format("{0} St  {1}/{2} Ave", street, avenue, avenue - 1),
                    StartsAt = this.intersections[street, avenue],
                    EndsAt = this.intersections[street, avenue - 1]
                };

                this.intersections[street, avenue].Blocks[Direction.East] = block;
                this.intersections[street, avenue - 1].Blocks[Direction.West] = block;
                this.blocks.Add(block);
            }
        }

        private void BuildUptownAvenue(int avenue)
        {
            for (int street = 1; street < NumStreets; ++street)
            {
                var block = new Block
                                {
                                    LocationCount = 5,
                                    StreetDirection = Direction.North,
                                    Name = string.Format("{0} avenue between {1} and {2} street", avenue, street, street + 1),
                                    ShortName = string.Format("{0} Ave {1}/{2} St", avenue, street, street + 1),
                                    StartsAt = this.intersections[street, avenue],
                                    EndsAt = this.intersections[street + 1, avenue]
                                };

                this.intersections[street, avenue].Blocks[Direction.North] = block;
                this.intersections[street + 1, avenue].Blocks[Direction.South] = block;
                this.blocks.Add(block);
            }
        }

        private void BuildDowntownAvenue(int avenue)
        {
            for (int street = 2; street <= NumStreets; ++street)
            {
                var block = new Block
                {
                    LocationCount = 5,
                    StreetDirection = Direction.South,
                    Name = string.Format("{0} avenue between {1} and {2} street", avenue, street, street - 1),
                    ShortName = string.Format("{0} Ave {1}/{2} St", avenue, street, street - 1),
                    StartsAt = this.intersections[street, avenue],
                    EndsAt = this.intersections[street - 1, avenue]
                };

                this.intersections[street, avenue].Blocks[Direction.South] = block;
                this.intersections[street - 1, avenue].Blocks[Direction.North] = block;
                this.blocks.Add(block);
            }
        }

        private void PopulateIntersections()
        {
            for (int street = 1; street <= NumStreets; ++street)
            {
                for (int avenue = 1; avenue <= NumAvenues; ++avenue)
                {
                    var intersection = new Intersection { Name = string.Format("{0} Street {1} Aveneue", street, avenue) };
                    this.intersections[street, avenue] = intersection;
                }
            }
        }

        private void UpdateRoadwork()
        {
            try
            {
                var client = new RoadworkServiceReference.RoadworkServiceClient();
                var locations = new List<RoadworkServiceReference.Block>();
                foreach (var item in this.blocks)
                {
                    RoadworkServiceReference.Direction direction;
                    Enum.TryParse(item.StreetDirection.ToString(), out direction);
                    var block = new RoadworkServiceReference.Block
                    {
                        Name = item.Name,
                        ShortName = item.ShortName,
                        StreetDirection = direction
                    };

                    locations.Add(block);
                }

                var impediments = client.RetrieveCurrent(locations.ToArray());
            }
            catch (Exception)
            {
                this.Run = false;
            }
        }

        private void OnTimer(object state)
        {
            int x = Interlocked.Increment(ref this.interlock1);
            try
            {
                if (x == 1)
                {
                    ++this.IterationCounter;
                    if (++this.phase >= 10)
                    {
                        this.UpdateRoadwork();
                        this.phase = 1;
                    }

                    this.MoveCars();
                }
            }
            finally
            {
                Interlocked.Decrement(ref this.interlock1);
            }
        }

        private void AddRandomRouteCar(Random r)
        {
            var car = new Car(new RandomTravel(), Brushes.LightBlue);
            int street = r.Next(2, NumStreets - 1);
            int avenue = r.Next(2, NumAvenues - 1);
            Direction way = r.Next(1, 2) == 1 ? Direction.West : Direction.North;
            var block = this.intersections[street, avenue].Blocks[way];
            int spot = r.Next(0, block.LocationCount - 1);
            var location = new ElementLocation { Road = block, Position = spot };
            car.Location = location;
            DiscoveredRoutes.ToRoutePart(block).Add(car);
            car.Routing.PickRoute(car);
            this.Cars.Add(car);
        }

        private void AddShortestDistanceCar(Random r)
        {
            var car = new Car(new ShortestDistance { StartTrip = this.StartTrip, EndTrip = this.EndTrip }, Brushes.Green);
            Direction way = r.Next(1, 2) == 1 ? Direction.West : Direction.North;
            var block = this.StartTrip.Blocks[way];
            int spot = 1;
            var location = new ElementLocation { Road = block, Position = spot };
            car.Location = location;
            DiscoveredRoutes.ToRoutePart(block).Add(car);
            car.Routing.PickRoute(car);
            this.Cars.Add(car);
        }

        private void AddShortestTimeCar(Random r)
        {
            var car = new Car(new ShortestTime { EndTrip = this.EndTrip, StartTrip = this.StartTrip }, Brushes.Red);
            Direction way = r.Next(1, 2) == 1 ? Direction.West : Direction.North;
            var block = this.StartTrip.Blocks[way];
            int spot = 1;
            var location = new ElementLocation { Road = block, Position = spot };
            car.Location = location;
            DiscoveredRoutes.ToRoutePart(block).Add(car);
            car.Routing.PickRoute(car);
            this.Cars.Add(car);
        }
        
        private void MoveCars()
        {
            foreach (var car in this.Cars)
            {
                var location = car.Location;
                if (location.Position < location.Road.LocationCount)
                {
                    if (car.ShouldMove)
                    {
                        ++location.Position;
                    }
                }
                else
                {
                    car.Routing.PickRoute(car);
                }

                // Trace.WriteLine(string.Format("Car At {0} position={1} Heading={2}", location.Road.Name, location.Position, location.Road.StreetDirection));
            }
        }
    }
}
