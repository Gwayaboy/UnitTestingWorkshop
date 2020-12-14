// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="LogAggregatorTests.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The LogAggregatorTests
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace EnterpriseLogger.Tests.Unit
{
    using System;
    using System.Fakes;
    using System.IO.Fakes;
    using Microsoft.ALMRangers.FakesGuide.EnterpriseLogger;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LogAggregatorTests
    {
        [TestMethod]
        public void AggregateLogs_PastThreeDays_ReturnsAllLinesFromPastThreeDays()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var sut = new LogAggregator();
                ShimDirectory.GetFilesStringString = (dir, pattern) => new[] 
                {
                    @"C:\someLogDir\Log_20121001.log",
                    @"C:\someLogDir\Log_20121002.log",
                    @"C:\someLogDir\Log_20121005.log"
                };
                ShimFile.ReadAllLinesString = path =>
                {
                    switch (path)
                    {
                        case @"C:\someLogDir\Log_20121001.log":
                            return new[] { "OctFirstLine1", "OctFirstLine2" };
                        case @"C:\someLogDir\Log_20121002.log":
                            return new[] { "ThreeDaysAgoFirstLine", "OctSecondLine2" };
                        case @"C:\someLogDir\Log_20121005.log":
                            return new[] { "OctFifthLine1", "TodayLastLine" };
                    }

                    return new string[] { };
                };
                ShimDateTime.TodayGet = () => new DateTime(2012, 10, 05);

                // Act
                var result = sut.AggregateLogs(@"C:\SomeLogDir", daysInPast: 3);
                
                // Assert
                Assert.AreEqual(4, result.Length, "Number of aggregated lines incorrect.");
                CollectionAssert.Contains(result, "ThreeDaysAgoFirstLine", "Expected line missing from aggregated log.");
                CollectionAssert.Contains(result, "TodayLastLine", "Expected line missing from aggregated log.");
            }
        }
    }
}
