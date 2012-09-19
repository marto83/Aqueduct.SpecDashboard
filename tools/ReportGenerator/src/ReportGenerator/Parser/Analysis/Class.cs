﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Palmmedia.ReportGenerator.Parser.Analysis
{
    /// <summary>
    /// Represents a class.
    /// </summary>
    public class Class
    {
        /// <summary>
        /// List of files that define this class.
        /// </summary>
        private readonly List<CodeFile> files = new List<CodeFile>();

        /// <summary>
        /// The method metrics of the class.
        /// </summary>
        private readonly List<MethodMetric> methodMetrics = new List<MethodMetric>();

        /// <summary>
        /// The coverage quota.
        /// </summary>
        private decimal? coverageQuota;

        /// <summary>
        /// Initializes a new instance of the <see cref="Class"/> class.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <param name="assembly">The assembly.</param>
        public Class(string name, Assembly assembly)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            this.Name = name;
            this.Assembly = assembly;
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public IEnumerable<CodeFile> Files
        {
            get
            {
                return this.files.OrderBy(f => f.Path);
            }
        }

        /// <summary>
        /// Gets the method metrics.
        /// </summary>
        /// <value>The method metrics.</value>
        public IEnumerable<MethodMetric> MethodMetrics
        {
            get
            {
                return this.methodMetrics;
            }
        }

        /// <summary>
        /// Gets the coverage type.
        /// </summary>
        /// <value>The coverage type.</value>
        public CoverageType CoverageType
        {
            get
            {
                return this.files.Count == 0 ? CoverageType.MethodCoverage : CoverageType.LineCoverage;
            }
        }

        /// <summary>
        /// Gets the number of covered lines.
        /// </summary>
        /// <value>The covered lines.</value>
        public int CoveredLines
        {
            get
            {
                return this.files.Sum(f => f.CoveredLines);
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
                return this.files.Sum(f => f.CoverableLines);
            }
        }

        /// <summary>
        /// Gets the number of total lines.
        /// </summary>
        /// <value>The total lines.</value>
        public int? TotalLines
        {
            get
            {
                return this.files.Sum(f => f.TotalLines);
            }
        }

        /// <summary>
        /// Gets or sets the coverage quota of the class.
        /// </summary>
        /// <value>The coverage quota.</value>
        public decimal CoverageQuota
        {
            get
            {
                if (this.files.Count == 0)
                {
                    return this.coverageQuota.GetValueOrDefault(0);
                }
                else
                {
                    return (this.CoverableLines == 0) ? 0 : (decimal)Math.Truncate(1000 * (double)this.CoveredLines / (double)this.CoverableLines) / 10;
                }
            }

            set
            {
                this.coverageQuota = value;
            }
        }

        /// <summary>
        /// Adds the given file.
        /// </summary>
        /// <param name="codeFile">The code file.</param>
        public void AddFile(CodeFile codeFile)
        {
            this.files.Add(codeFile);
        }

        /// <summary>
        /// Adds the given method metric.
        /// </summary>
        /// <param name="methodMetric">The method metric.</param>
        public void AddMethodMetric(MethodMetric methodMetric)
        {
            this.methodMetrics.Add(methodMetric);
        }

        /// <summary>
        /// Merges the given class with the current instance.
        /// </summary>
        /// <param name="class">The class to merge.</param>
        public void Merge(Class @class)
        {
            if (@class == null)
            {
                throw new ArgumentNullException("class");
            }

            if (this.coverageQuota.HasValue && @class.coverageQuota.HasValue)
            {
                this.CoverageQuota = Math.Max(this.coverageQuota.Value, @class.coverageQuota.Value);
            }
            else if (@class.coverageQuota.HasValue)
            {
                this.CoverageQuota = @class.coverageQuota.Value;
            }

            foreach (var methodMetric in @class.methodMetrics)
            {
                var existingMethodMetric = this.methodMetrics.FirstOrDefault(m => m.Name == methodMetric.Name);
                if (existingMethodMetric != null)
                {
                    existingMethodMetric.Merge(methodMetric);
                }
                else
                {
                    this.AddMethodMetric(methodMetric);
                }
            }

            foreach (var file in @class.files)
            {
                var existingFile = this.files.FirstOrDefault(f => f.Path == file.Path);
                if (existingFile != null)
                {
                    existingFile.Merge(file);
                }
                else
                {
                    this.AddFile(file);
                }
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
            if (obj == null || !obj.GetType().Equals(typeof(Class)))
            {
                return false;
            }
            else
            {
                var @class = (Class)obj;
                return @class.Name.Equals(this.Name) && @class.Assembly.Equals(this.Assembly);
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
            return this.Name.GetHashCode();
        }
    }
}
