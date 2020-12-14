// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="LogAggregator.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The LogAggregator
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.FakesGuide.EnterpriseLogger
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    public class LogAggregator
    {
        public string[] AggregateLogs(string logDirPath, int daysInPast)
        {
            var mergedLines = new List<string>();
            var filePaths = Directory.GetFiles(logDirPath, "*.log");
            foreach (var filePath in filePaths)
            {
                if (this.IsInDateRange(filePath, daysInPast))
                {
                    mergedLines.AddRange(File.ReadAllLines(filePath));
                }
            }

            return mergedLines.ToArray();
        }

        private bool IsInDateRange(string filePath, int daysInPast)
        {
            string logName = Path.GetFileNameWithoutExtension(filePath);
            if (logName.Length < 8)
            {
                return false;
            }

            string logDayString = logName.Substring(logName.Length - 8, 8);
            DateTime logDay;
            DateTime today = DateTime.Today;
            if (DateTime.TryParseExact(logDayString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out logDay))
            {
                return logDay.AddDays(daysInPast) >= today;
            }

            return false;
        }
    }
}
