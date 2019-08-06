using Microsoft.ALMRangers.FakesGuide.EnterpriseLogger;
using Microsoft.QualityTools.Testing.Fakes;
using NUnit.Framework;
using System;
using System.Fakes;
using System.IO.Fakes;

namespace EntrepriseLogger.UnitTests
{
    [TestFixture]
    public class LogAggregatorTests
    {
        [Test]
        public void AggregatesAllLogsForTheLastThreedays()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var sut = new LogAggregator();

                ShimDirectory.GetFilesStringString = (path, seachPattern) =>
                new[]
                {
                    @"C:\someLogDir\Log_20180801.log",
                    @"C:\someLogDir\Log_20180827.log",
                    @"C:\someLogDir\Log_20180829.log"
                };
                ShimFile.ReadAllLinesString = path =>
                {
                    switch (path)
                    {
                        case @"C:\someLogDir\Log_20180801.log":
                            return new[] { "OctFirstLine1", "OctFirstLine2" };

                        case @"C:\someLogDir\Log_20180827.log":
                            return new[] { "2DaysAgoPickedUpLine", "SOmethingElse" };

                        case @"C:\someLogDir\Log_20180829.log":
                            return new[] { "OctFifthLine1", "TodayLastLine" };

                        default:
                            return new string[] { };
                    }
                };


                ShimDateTime.TodayGet = () => new DateTime(2018, 08, 29);
                // Act
                var logResults = sut.AggregateLogs(@"C:\SomeDir", 3);

                // Assert
                Assert.AreEqual(4, logResults.Length);
                Assert.Contains("2DaysAgoPickedUpLine", logResults);
                Assert.Contains("TodayLastLine", logResults);
            }
        }
    }
}
