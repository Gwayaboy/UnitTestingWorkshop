// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.UI
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Microsoft.ALMRangers.FakesGuide.ComplexDependencies.Traffic.Core.Models;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<Ellipse> blips = new List<Ellipse>();
        private readonly Rectangle[,] streets = new Rectangle[City.NumStreets + 1, City.NumAvenues + 1];
        private readonly Rectangle[,] avenues = new Rectangle[City.NumStreets + 1, City.NumAvenues + 1];
        private City city;
        private int interlock1;
        private int interlock2;
        private Timer timer;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            this.timer = new Timer(this.OnTimer, null, 0, 250);
        }

        private void InitializeCity()
        {
            this.city = new City();
            this.city.BuildCity();
            this.city.InitializeRoutes();
            this.city.InitializeCars();
            this.city.Run = true;
            Dispatcher.Invoke(() => { this.DrawStreets(); this.DrawCars(); });
        }

        private void DrawCars()
        {
            foreach (var blip in this.blips)
            {
                Map.Children.Remove(blip);
            }

            this.blips.Clear();
            for (int street = 1; street <= City.NumStreets; ++street)
            {
                for (int avenue = 1; avenue <= City.NumAvenues; ++avenue)
                {
                    this.DrawBlock(street, avenue, Direction.West);
                    this.DrawBlock(street, avenue, Direction.North);
                }
            }
        }

        private void DrawBlock(int street, int avenue, Direction direction)
        {
            var model = this.city.Intersections[street, avenue].Blocks[direction];
            if (model != null)
            {
                var view = (direction == Direction.West) ? this.streets[street, avenue] : this.avenues[street, avenue];
                var cars = new List<Car>(model.Occupants);
                foreach (var car in cars)
                {
                    this.DrawCar(car, view);
                }
            }
        }

        private void DrawCar(Car car, Rectangle view)
        {
            Block block = car.Location.Road;
            var blip = new Ellipse
                           {
                               Width = 10, 
                               Height = 10, 
                               Visibility = Visibility.Visible,
                               Fill = car.VehicleColor
                           };
            double xvalue = 0.0;
            double yvalue = 0.0;
            double offset = 2;
            switch (car.Location.Road.StreetDirection)
            {
                case Direction.West:
                    xvalue = ((double)view.GetValue(Canvas.RightProperty)) + ((car.Location.Position * (view.Width - 15)) / block.LocationCount) + offset;
                    yvalue = ((double)view.GetValue(Canvas.BottomProperty)) + offset;
                    break;
                case Direction.East:
                    xvalue = ((double)view.GetValue(Canvas.RightProperty)) + ((car.Location.Road.LocationCount - car.Location.Position) * (view.Width - 15) / block.LocationCount) + offset;
                    yvalue = ((double)view.GetValue(Canvas.BottomProperty)) + offset;
                    break;
                case Direction.North:
                    xvalue = ((double)view.GetValue(Canvas.RightProperty)) + offset;
                    yvalue = ((double)view.GetValue(Canvas.BottomProperty)) + (car.Location.Position * (view.Height - 15) / block.LocationCount) + offset;
                    break;
                case Direction.South:
                    xvalue = ((double)view.GetValue(Canvas.RightProperty)) + offset;
                    yvalue = ((double)view.GetValue(Canvas.BottomProperty)) + ((car.Location.Road.LocationCount - car.Location.Position) * (view.Height - 15) / block.LocationCount) + offset;
                    break;
            }

            blip.SetValue(Canvas.RightProperty, xvalue);
            blip.SetValue(Canvas.BottomProperty, yvalue);
            this.blips.Add(blip);
            Map.Children.Add(blip);
        }

        private void DrawStreets()
        {
            double streetSpacing = this.Height / (City.NumStreets + 3);
            double avenueSpacing = this.Width / (City.NumAvenues + 1);
            for (int street = 1; street <= City.NumStreets; ++street)
            {
                for (int avenue = 1; avenue <= City.NumAvenues; ++avenue)
                {
                    if (avenue < City.NumAvenues)
                    {
                        Rectangle crossStreet = new Rectangle();
                        crossStreet.Fill = Brushes.DarkGray;
                        crossStreet.Height = 15;
                        crossStreet.Width = avenueSpacing + 15;
                        crossStreet.SetValue(Canvas.RightProperty, avenue * avenueSpacing);
                        crossStreet.SetValue(Canvas.BottomProperty, street * streetSpacing);
                        Map.Children.Add(crossStreet);
                        Intersection intersection = this.city.Intersections[street, avenue];
                        if (intersection != null)
                        {
                            var model = intersection.Blocks[Direction.West];
                            if (model != null)
                            {
                                TextBlock streetSign = new TextBlock();
                                streetSign.Text = model.ShortName;
                                streetSign.FontSize = 9;
                                streetSign.SetValue(Canvas.RightProperty, (avenue + 0.5) * avenueSpacing);
                                streetSign.SetValue(Canvas.BottomProperty, street * streetSpacing);
                                Map.Children.Add(streetSign);
                            }
                        }

                        this.streets[street, avenue] = crossStreet;
                    }

                    if (street < City.NumStreets)
                    {
                        Rectangle upstreet = new Rectangle();
                        upstreet.Fill = Brushes.DarkGray;
                        upstreet.Height = streetSpacing + 15;
                        upstreet.Width = 15;
                        upstreet.SetValue(Canvas.RightProperty, avenue * avenueSpacing);
                        upstreet.SetValue(Canvas.BottomProperty, street * streetSpacing);
                        Map.Children.Add(upstreet);
                        Intersection intersection = this.city.Intersections[street, avenue];
                        if (intersection != null)
                        {
                            var model = intersection.Blocks[Direction.North];
                            if (model != null)
                            {
                                TextBlock streetSign = new TextBlock();
                                streetSign.Text = model.ShortName;
                                streetSign.FontSize = 7;
                                streetSign.RenderTransform = new RotateTransform(90);  
                                streetSign.SetValue(Canvas.RightProperty, (avenue - 0.20) * avenueSpacing);
                                streetSign.SetValue(Canvas.BottomProperty, street * streetSpacing);
                                Map.Children.Add(streetSign);
                            }
                        }

                        this.avenues[street, avenue] = upstreet;
                    }
                }
            }
        }

        private void OnTimer(object state)
        {
            int x1 = Interlocked.Increment(ref this.interlock1);
            try
            {
                if (x1 == 1)
                {
                    if (this.city == null)
                    {
                        this.InitializeCity();
                    }

                    Dispatcher.Invoke(() =>
                                          {
                                              int x2 = Interlocked.Increment(ref interlock2);
                                              try
                                              {
                                                  if (x2 == 1)
                                                  {
                                                      txtClock.Text = DateTime.Now.ToString("HH:mm:ss.fff");
                                                      DrawCars();
                                                  }
                                              }
                                              finally
                                              {
                                                  Interlocked.Decrement(ref interlock2);
                                              }
                                          });
                }
            }
            finally
            {
                Interlocked.Decrement(ref this.interlock1);
            }
        }
    }
}
