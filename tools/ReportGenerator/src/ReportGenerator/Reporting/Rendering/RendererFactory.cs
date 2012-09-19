using System;
using System.Collections.Generic;

namespace Palmmedia.ReportGenerator.Reporting.Rendering
{
    /// <summary>
    /// Default factory for <see cref="IReportRenderer"/>.
    /// </summary>
    internal class RendererFactory : IRendererFactory
    {
        /// <summary>
        /// Type of the report.
        /// </summary>
        private readonly ReportTypes reportType;

        /// <summary>
        /// The cached renderer. Required for reusing an <see cref="IReportRenderer"/> for generating reports containing all classes.
        /// </summary>
        private IReportRenderer cachedRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererFactory"/> class.
        /// </summary>
        /// <param name="reportType">Type of the report.</param>
        public RendererFactory(ReportTypes reportType)
        {
            this.reportType = reportType;
        }

        /// <summary>
        /// Gets a value indicating whether class reports can be generated in parallel.
        /// </summary>
        /// <value></value>
        public bool SupportsParallelClassReports
        {
            get
            {
                return !this.reportType.HasFlag(ReportTypes.Latex);
            }
        }

        /// <summary>
        /// Creates the renderer for the summary report.
        /// </summary>
        /// <returns>The renderer for the summary report.</returns>
        public IReportRenderer CreateSummaryRenderer()
        {
            var renderers = new List<IReportRenderer>();

            if (this.reportType.HasFlag(ReportTypes.HtmlSummary)
                || this.reportType.HasFlag(ReportTypes.Html))
            {
                renderers.Add(new HtmlRenderer());
            }
            
            if (this.reportType.HasFlag(ReportTypes.XmlSummary)
                || this.reportType.HasFlag(ReportTypes.Xml))
            {
                renderers.Add(new XmlRenderer());
            }
            
            if (this.reportType.HasFlag(ReportTypes.Latex)
                || this.reportType.HasFlag(ReportTypes.LatexSummary))
            {
                if (this.cachedRenderer == null)
                {
                    this.cachedRenderer = new LatexRenderer();
                }

                renderers.Add(this.cachedRenderer);
            }

            return new MultiRenderer(renderers);
        }

        /// <summary>
        /// Creates the renderer for the class report.
        /// </summary>
        /// <returns>The renderer for the class report.</returns>
        public IReportRenderer CreateClassRenderer()
        {
            var renderers = new List<IReportRenderer>();

            if (this.reportType.HasFlag(ReportTypes.Html))
            {
                renderers.Add(new HtmlRenderer());
            }
            
            if (this.reportType.HasFlag(ReportTypes.Xml))
            {
                renderers.Add(new XmlRenderer());
            }
            
            if (this.reportType.HasFlag(ReportTypes.Latex))
            {
                if (this.cachedRenderer == null)
                {
                    this.cachedRenderer = new LatexRenderer();
                }

                renderers.Add(this.cachedRenderer);
            }

            return new MultiRenderer(renderers);
        }
    }
}
