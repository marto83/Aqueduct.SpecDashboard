using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Aqueduct.SpecDashboard.Models;
using System.IO;
using Ionic.Zip;

namespace Aqueduct.SpecDashboard.Data
{
    public class ReportRepository
    {
        public IEnumerable<Report> GetReports()
        {
            var reportPaths = Directory.GetDirectories(Settings.ReportsFolder);

            var result = new List<Report>();
            foreach (var path in reportPaths)
            {
                result.Add(LoadReport(path));
            }

            return result;
        }

        private XDocument LoadXml(string filePath)
        {

            using (Stream xmlStream = File.OpenRead(filePath))
            {
                var readerSettings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore
                };
                using (var reader = XmlReader.Create(xmlStream, readerSettings))
                {
                    return XDocument.Load(reader);
                }
            }
        }

        private Report LoadReport(string path)
        {
            var report = new Report(Path.GetFileNameWithoutExtension(path));
            IList<ReportVersion> getVersions =  GetVersions(path);
            var latest = getVersions.OrderByDescending(x => x.Date).First();
            var previous = getVersions.OrderByDescending(x => x.Date).ElementAt(1);
            report.Versions = getVersions;


   
            //// Load test results xml from the latest version
            //var xmlPath = Path.Combine(latest.Path, "TestResult.xml");
            //var document = LoadXml(xmlPath);

            //var total = document.Root.Attributes().First(x => x.Name == "total").Value;
            //int noOfTests = int.Parse(total);
            //report.NumberOfTests = noOfTests;

            //var errors = document.Root.Attributes().First(x => x.Name == "total").Value;
            //int noOfErrors = int.Parse(errors);
            //report.Errors = noOfErrors;

            //var failures = document.Root.Attributes().First(x => x.Name == "failures").Value;
            //int noOfFailures = int.Parse(failures);
            //report.Failures = noOfFailures;

            //var inconclusive = document.Root.Attributes().First(x => x.Name == "inconclusive").Value;
            //int noOfInconclusive = int.Parse(inconclusive);
            //report.Inconclusive = noOfInconclusive;

            //int success = noOfTests - noOfFailures- noOfInconclusive;
            //report.Success = success;

            //double percentageSuccess = ((double)success/(double)noOfTests)*100;
            //var percentageSuccessRound = Math.Round(percentageSuccess, 0);
            //int percentSuccess = Convert.ToInt32(percentageSuccessRound);
            //report.PercentageSuccess = percentSuccess;

            //var date = document.Root.Attributes().First(x => x.Name == "date").Value;
            //report.Date = date;

            //var time = document.Root.Attributes().First(x => x.Name == "time").Value;
            //report.Time = time;

            //double percentageInconclusive = ((double)noOfInconclusive / (double)noOfTests) * 100;
            //var percentageInconclusiveRound = Math.Round(percentageInconclusive, 0);
            //int percentInconclusive = Convert.ToInt32(percentageInconclusiveRound);
            //report.PercentageInconclusive = percentInconclusive;

            //double percentageFailed = ((double)noOfFailures / (double)noOfTests) * 100;
            //var percentageFailedRound = Math.Round(percentageFailed, 0);
            //int percentFail = Convert.ToInt32(percentageFailedRound);
            //report.PercentageFailure = percentFail;


            //if(change > 0)
            //{
            //    report.PercentageSuccessDifference = 1;
            //}
            //else if(change == 0)
            //{
            //    report.PercentageSuccessDifference = 0;
            //}
            //else
            //{
            //    report.PercentageSuccessDifference = -1;
            //}

            return report;
        }

        private IList<ReportVersion> GetVersions(string path)
        {
            var results = new List<ReportVersion>();
            foreach (var versionPath in Directory.GetDirectories(path))
            {
                var info = new DirectoryInfo(versionPath);
                var version = new ReportVersion() {Version = info.Name, Date = info.CreationTime, Path = info.FullName};
                version.Statistics = CalculateStatistics(version);
                results.Add(version);
            }
            return results;
        }

        private ReportTestStatistics CalculateStatistics(ReportVersion version)
        {
            var statistics = new ReportTestStatistics();
            
            //Calucate all the stats 
            // Load test results xml from the latest version
            var xmlPath = Path.Combine(version.Path, "TestResult.xml");
            var document = LoadXml(xmlPath);

            var total = document.Root.Attributes().First(x => x.Name == "total").Value;
            int noOfTests = int.Parse(total);
            statistics.NumberOfTests = noOfTests;

            var failures = document.Root.Attributes().First(x => x.Name == "failures").Value;
            int noOfFailures = int.Parse(failures);
            statistics.Failures = noOfFailures;

            var inconclusive = document.Root.Attributes().First(x => x.Name == "inconclusive").Value;
            int noOfInconclusive = int.Parse(inconclusive);
            statistics.Inconclusive = noOfInconclusive;

            int success = noOfTests - noOfFailures - noOfInconclusive;
            statistics.Success = success;

            return statistics;
        }

        public Report FindReport(string id)
        {
            return GetReports().FirstOrDefault(x => x.Id == id);
        }

        private string GetReportVersionPath(string id, string version)
        {
            string directory = Path.Combine(Settings.ReportsFolder, id, version);
            if (Directory.Exists(directory))
            {
                var report = FindReport(id);
                var latestVersion = Version.Parse(report.Latest.Version);
                var newVersion = new Version(latestVersion.Major, latestVersion.Minor, latestVersion.Build, latestVersion.Revision + 1);
                directory = Path.Combine(Settings.ReportsFolder, id, newVersion.ToString());
            }
            return directory;
        }

        public void AddReport(string id, string version, Stream inputStream)
        {
            var directory = GetReportVersionPath(id, version);
            
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);

            Directory.CreateDirectory(directory);
            string zipFileName = Path.Combine(directory, "documentation.zip");
            using (var stream = File.Open(zipFileName, FileMode.Create))
            {
                inputStream.CopyTo(stream);
            }

            using (var zip = ZipFile.Read(zipFileName))
            {
                zip.ExtractAll(directory);
            }
        }
    }
}