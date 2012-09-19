﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis;

namespace Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch
{
    /// <summary>
    /// Searches several directories for class files.
    /// </summary>
    internal class MultiDirectoryClassSearcher : ClassSearcher
    {
        /// <summary>
        /// The <see cref="ClassSearcher">ClassSearchers</see>.
        /// </summary>
        private readonly IEnumerable<ClassSearcher> classSearchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDirectoryClassSearcher"/> class.
        /// </summary>
        /// <param name="classSearchers">The <see cref="ClassSearcher">ClassSearchers</see>.</param>
        public MultiDirectoryClassSearcher(IEnumerable<ClassSearcher> classSearchers)
        {
            if (classSearchers == null)
            {
                throw new ArgumentNullException("classSearchers");
            }

            this.classSearchers = classSearchers;
        }

        /// <summary>
        /// Gets the files the given class is defined in.
        /// </summary>
        /// <param name="className">Name of the class (with full namespace).</param>
        /// <returns>The files the class is defined in.</returns>
        public override IEnumerable<string> GetFilesOfClass(string className)
        {
            return this.classSearchers
                .SelectMany(c => c.GetFilesOfClass(className))
                .Distinct();
        }
    }
}