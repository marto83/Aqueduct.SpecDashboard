﻿using System.Collections.Generic;
using System.IO;
using log4net;
using Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch
{
    /// <summary>
    /// Searches one directory for class files.
    /// </summary>
    internal class ClassSearcher
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(ClassSearcher));

        /// <summary>
        /// Dictionary containing the files a class is defined in by its classname.
        /// </summary>
        private readonly Dictionary<string, HashSet<string>> filesByClassName = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Indicates whether file search was executed.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassSearcher"/> class.
        /// </summary>
        /// <param name="directory">The directory that should be searched for class files.</param>
        public ClassSearcher(string directory)
        {
            this.Directory = directory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassSearcher"/> class.
        /// </summary>
        protected ClassSearcher()
        {
        }

        /// <summary>
        /// Gets the directory that should be searched for class files.
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Gets the files the given class is defined in.
        /// </summary>
        /// <param name="className">Name of the class (with full namespace).</param>
        /// <returns>The files the class is defined in.</returns>
        public virtual IEnumerable<string> GetFilesOfClass(string className)
        {
            if (!this.initialized)
            {
                this.SearchClassFiles();
                this.initialized = true;
            }

            HashSet<string> filesOfClass;

            if (this.filesByClassName.TryGetValue(className, out filesOfClass))
            {
                return filesOfClass;
            }
            else
            {
                return new string[] { };
            }
        }

        /// <summary>
        /// Searches the class files.
        /// </summary>
        private void SearchClassFiles()
        {
            if (!System.IO.Directory.Exists(this.Directory))
            {
                return;
            }

            logger.DebugFormat("  " + Resources.IndexingClasses, new DirectoryInfo(this.Directory).FullName);

            foreach (var file in System.IO.Directory.EnumerateFiles(this.Directory, "*.cs", SearchOption.AllDirectories))
            {
                foreach (var classInFile in SourceCodeAnalyzer.GetClassesInFile(file))
                {
                    HashSet<string> filesOfClass = null;

                    if (!this.filesByClassName.TryGetValue(classInFile, out filesOfClass))
                    {
                        filesOfClass = new HashSet<string>();
                        this.filesByClassName.Add(classInFile, filesOfClass);
                    }

                    filesOfClass.Add(file);
                }
            }
        }
    }
}