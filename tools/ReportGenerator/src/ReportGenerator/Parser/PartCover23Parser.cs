﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Palmmedia.ReportGenerator.Common;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Parser
{
    /// <summary>
    /// Parser for XML reports generated by PartCover 2.3 and above.
    /// </summary>
    public class PartCover23Parser : ParserBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(PartCover23Parser));

        /// <summary>
        /// Dictionary containing the assembly names by id.
        /// In PartCover 2.3.0.35109 the assemblies are referenced by an id.
        /// Before only their name was required.
        /// </summary>
        private Dictionary<string, string> assembliesByIdDictionary;

        /// <summary>
        /// Dictionary containing the file ids by the file's path.
        /// </summary>
        private Dictionary<string, string> fileIdByFilenameDictionary;

        /// <summary>
        /// The type elements of the report.
        /// </summary>
        private XElement[] types;

        /// <summary>
        /// The file elements of the report.
        /// </summary>
        private XElement[] files;

        /// <summary>
        /// The attribute name to the corresponding assembly.
        /// In PartCover 2.3.0.35109 this is "asmref".
        /// </summary>
        private string assemblyAttribute = "asm";

        /// <summary>
        /// Initializes a new instance of the <see cref="PartCover23Parser"/> class.
        /// </summary>
        /// <param name="report">The report file as XContainer.</param>
        public PartCover23Parser(XContainer report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            this.types = report.Descendants("Type").ToArray();
            this.files = report.Descendants("File").ToArray();

            // Determine which version of PartCover 2.3 has been used.
            // In PartCover 2.3.0.35109 the assemblies are referenced by an id and the attribute name in Type elements has changed.
            var assemblies = report.Descendants("Assembly");
            if (assemblies.Any() && assemblies.First().Attribute("id") != null)
            {
                this.assemblyAttribute = "asmref";
                this.assembliesByIdDictionary = assemblies.ToDictionary(a => a.Attribute("id").Value, a => a.Attribute("name").Value);
            }
            else
            {
                this.assembliesByIdDictionary = assemblies.ToDictionary(a => a.Attribute("name").Value, a => a.Attribute("name").Value);
            }

            this.fileIdByFilenameDictionary = this.files.ToDictionary(f => f.Attribute("url").Value, f => f.Attribute("id").Value);

            var assemblyNames = this.assembliesByIdDictionary.Values
                .Distinct()
                .OrderBy(a => a)
                .ToArray();

            Parallel.ForEach(assemblyNames, assemblyName => this.AddAssembly(this.ProcessAssembly(assemblyName)));

            this.types = null;
            this.files = null;
            this.assembliesByIdDictionary = null;
            this.fileIdByFilenameDictionary = null;
        }

        /// <summary>
        /// Processes the given assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The <see cref="Assembly"/>.</returns>
        private Assembly ProcessAssembly(string assemblyName)
        {
            logger.DebugFormat("  " + Resources.CurrentAssembly, assemblyName);

            var classNames = this.types
                .Where(type => this.assembliesByIdDictionary[type.Attribute(this.assemblyAttribute).Value].Equals(assemblyName) && !type.Attribute("name").Value.Contains("__"))
                .Select(type => type.Attribute("name").Value)
                .OrderBy(name => name)
                .Distinct()
                .ToArray();

            var assembly = new Assembly(assemblyName);

            Parallel.ForEach(classNames, className => assembly.AddClass(this.ProcessClass(assembly, className)));

            return assembly;
        }

        /// <summary>
        /// Processes the given class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns>The <see cref="Class"/>.</returns>
        private Class ProcessClass(Assembly assembly, string className)
        {
            var fileIdsOfClass = this.types
                .Where(type => this.assembliesByIdDictionary[type.Attribute(this.assemblyAttribute).Value].Equals(assembly.Name) && type.Attribute("name").Value.Equals(className))
                .Elements("Method")
                .Elements("pt")
                .Where(pt => pt.Attribute("fid") != null)
                .Select(pt => pt.Attribute("fid").Value)
                .Distinct().ToHashSet();

            var filesOfClass = this.files
                .Where(file => fileIdsOfClass.Contains(file.Attribute("id").Value))
                .Select(file => file.Attribute("url").Value)
                .ToArray();

            var @class = new Class(className, assembly);

            foreach (var file in filesOfClass)
            {
                @class.AddFile(this.ProcessFile(@class, file));
            }

            return @class;
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="CodeFile"/>.</returns>
        private CodeFile ProcessFile(Class @class, string filePath)
        {
            string fileId = this.fileIdByFilenameDictionary[filePath];

            var seqpntsOfFile = this.types
                .Where(type => this.assembliesByIdDictionary[type.Attribute(this.assemblyAttribute).Value].Equals(@class.Assembly.Name)
                    && type.Attribute("name").Value.StartsWith(@class.Name, StringComparison.Ordinal))
                .Elements("Method")
                .Elements("pt")
                .Where(seqpnt => seqpnt.HasAttributeWithValue("fid", fileId))
                .Select(seqpnt => new
                {
                    LineNumber = int.Parse(seqpnt.Attribute("sl").Value, CultureInfo.InvariantCulture),
                    Visits = int.Parse(seqpnt.Attribute("visit").Value, CultureInfo.InvariantCulture)
                })
                .OrderBy(seqpnt => seqpnt.LineNumber)
                .ToArray();

            int[] coverage = new int[] { };

            if (seqpntsOfFile.Length > 0)
            {
                coverage = new int[seqpntsOfFile[seqpntsOfFile.LongLength - 1].LineNumber + 1];

                for (int i = 0; i < coverage.Length; i++)
                {
                    coverage[i] = -1;
                }

                foreach (var seqpnt in seqpntsOfFile)
                {
                    coverage[seqpnt.LineNumber] = coverage[seqpnt.LineNumber] == -1 ? seqpnt.Visits : coverage[seqpnt.LineNumber] + seqpnt.Visits;
                }
            }

            return new CodeFile(filePath, coverage);
        }
    }
}