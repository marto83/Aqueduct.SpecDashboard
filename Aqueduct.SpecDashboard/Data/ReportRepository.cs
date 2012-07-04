using System;
using System.Collections.Generic;
using System.Linq;
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

        private Report LoadReport(string path)
        {
            var report = new Report(Path.GetFileNameWithoutExtension(path));
            report.Versions = GetVersions(path);
            return report;
        }

        private IList<ReportVersion> GetVersions(string path)
        {
            var results = new List<ReportVersion>();
            foreach (var versionPath in Directory.GetDirectories(path))
            {
                var info = new DirectoryInfo(versionPath);
                results.Add(new ReportVersion() { Version = info.Name, Date = info.CreationTime });
            }
            return results;
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