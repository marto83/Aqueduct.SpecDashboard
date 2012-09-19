using System;
using log4net;
using Palmmedia.ReportGenerator.Parser;
using Palmmedia.ReportGenerator.Properties;
using Palmmedia.ReportGenerator.Reporting;
using Palmmedia.ReportGenerator.Reporting.Rendering;

namespace Palmmedia.ReportGenerator
{
    /// <summary>
    /// Command line access to the ReportBuilder.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Executes the report generation.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns><c>true</c> if report was generated successfully; otherwise <c>false</c>.</returns>
        public static bool Execute(ReportConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            var appender = new log4net.Appender.ConsoleAppender();
            appender.Layout = new log4net.Layout.PatternLayout("%message%newline");
            log4net.Config.BasicConfigurator.Configure(appender);

            if (!configuration.Validate())
            {
                return false;
            }

            if (configuration.VerbosityLevel == VerbosityLevel.Info)
            {
                appender.Threshold = log4net.Core.Level.Info;
            }
            else if (configuration.VerbosityLevel == VerbosityLevel.Error)
            {
                appender.Threshold = log4net.Core.Level.Error;
            }

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            var parser = ParserFactory.CreateParser(configuration.ReportFiles, configuration.SourceDirectories);

            new ReportBuilder(parser, new RendererFactory(configuration.ReportType), configuration.TargetDirectory, new DefaultAssemblyFilter(configuration.Filters)).CreateReport();

            stopWatch.Stop();
            logger.InfoFormat(Resources.ReportGenerationTook, stopWatch.ElapsedMilliseconds / 1000);

            return true;
        }

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>Return code indicating success/failure.</returns>
        internal static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                ReportConfigurationBuilder.ShowHelp();
                return 1;
            }

            ReportConfiguration configuration = ReportConfigurationBuilder.Create(args);

            return Execute(configuration) ? 0 : 1;
        }
    }
}
