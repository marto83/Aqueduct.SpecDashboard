﻿using System;
using ICSharpCode.NRefactory.PatternMatching;

namespace Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis
{
    /// <summary>
    /// Represents an element in a source code file.
    /// </summary>
    internal abstract class SourceElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceElement"/> class.
        /// </summary>
        /// <param name="classname">The classname.</param>
        public SourceElement(string classname)
        {
            if (classname == null)
            {
                throw new ArgumentNullException("classname");
            }

            this.Classname = classname;
        }

        /// <summary>
        /// Gets the classname.
        /// </summary>
        /// <value>The classname.</value>
        public string Classname { get; private set; }

        /// <summary>
        /// Determines whether the given <see cref="ICSharpCode.NRefactory.Ast.INode"/> matches the <see cref="SourceElement"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>A <see cref="SourceElementPosition"/> or <c>null</c> if <see cref="SourceElement"/> does not match the <see cref="ICSharpCode.NRefactory.Ast.INode"/>.</returns>
        public abstract SourceElementPosition GetSourceElementPosition(INode node);
    }
}
