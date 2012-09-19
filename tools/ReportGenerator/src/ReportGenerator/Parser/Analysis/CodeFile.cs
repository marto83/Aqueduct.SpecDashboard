using System;
using System.Globalization;
using System.IO;
using System.Linq;
using log4net;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Parser.Analysis
{
    /// <summary>
    /// Represents a source code file.
    /// </summary>
    public class CodeFile
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(CodeFile));

        /// <summary>
        /// Array containing the coverage information by line number.
        /// -1: Not coverable
        /// 0: Not visited
        /// >0: Number of visits
        /// </summary>
        private int[] lineCoverage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFile"/> class.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="lineCoverage">The line coverage.</param>
        public CodeFile(string path, int[] lineCoverage)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (lineCoverage == null)
            {
                throw new ArgumentNullException("lineCoverage");
            }

            this.Path = path;
            this.lineCoverage = lineCoverage;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the number of covered lines.
        /// </summary>
        /// <value>The covered lines.</value>
        public int CoveredLines
        {
            get
            {
                return this.lineCoverage.Count(l => l > 0);
            }
        }

        /// <summary>
        /// Gets the number of coverable lines.
        /// </summary>
        /// <value>The coverable lines.</value>
        public int CoverableLines
        {
            get
            {
                return this.lineCoverage.Count(l => l >= 0);
            }
        }

        /// <summary>
        /// Gets the total lines.
        /// </summary>
        /// <value>The total lines.</value>
        public int? TotalLines { get; private set; }

        /// <summary>
        /// Performs the analysis of the source file.
        /// </summary>
        /// <returns>The analysis result.</returns>
        public FileAnalysis AnalyzeFile()
        {
            if (!System.IO.File.Exists(this.Path))
            {
                string error = string.Format(CultureInfo.InvariantCulture, " " + Resources.FileDoesNotExist, this.Path);
                logger.Error(error);
                return new FileAnalysis(this.Path, error);
            }

            try
            {
                string[] lines = System.IO.File.ReadAllLines(this.Path);

                this.TotalLines = lines.Length;

                int currentLineNumber = 0;

                var result = new FileAnalysis(this.Path);

                foreach (var line in lines)
                {
                    currentLineNumber++;
                    int visits = this.lineCoverage.Length > currentLineNumber ? this.lineCoverage[currentLineNumber] : -1;

                    result.AddLineAnalysis(new LineAnalysis(visits, currentLineNumber, line.TrimEnd()));
                }

                return result;
            }
            catch (IOException ex)
            {
                string error = string.Format(CultureInfo.InvariantCulture, " " + Resources.ErrorDuringReadingFile, this.Path, ex.Message);
                logger.Error(error);
                return new FileAnalysis(this.Path, error);
            }
        }

        /// <summary>
        /// Merges the given file with the current instance.
        /// </summary>
        /// <param name="file">The file to merge.</param>
        public void Merge(CodeFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            // Resize coverage array if neccessary
            if (file.lineCoverage.LongLength > this.lineCoverage.LongLength)
            {
                int[] newLineCoverage = new int[file.lineCoverage.LongLength];

                Array.Copy(this.lineCoverage, newLineCoverage, this.lineCoverage.LongLength);

                for (long i = this.lineCoverage.LongLength; i < file.lineCoverage.LongLength; i++)
                {
                    newLineCoverage[i] = -1;
                }

                this.lineCoverage = newLineCoverage;
            }

            for (long i = 0; i < file.lineCoverage.LongLength; i++)
            {
                int coverage = this.lineCoverage[i];

                if (coverage < 0)
                {
                    coverage = file.lineCoverage[i];
                }
                else if (file.lineCoverage[i] > 0)
                {
                    coverage += file.lineCoverage[i];
                }

                this.lineCoverage[i] = coverage;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !obj.GetType().Equals(typeof(CodeFile)))
            {
                return false;
            }
            else
            {
                var codeFile = (CodeFile)obj;
                return codeFile.Path.Equals(this.Path);
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }
    }
}
