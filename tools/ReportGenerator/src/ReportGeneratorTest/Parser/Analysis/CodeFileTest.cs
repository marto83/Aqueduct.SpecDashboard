﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Palmmedia.ReportGenerator.Parser.Analysis;

namespace Palmmedia.ReportGeneratorTest.Parser.Analysis
{
    /// <summary>
    /// This is a test class for CodeFile and is intended
    /// to contain all CodeFile Unit Tests
    /// </summary>
    [TestClass]
    public class CodeFileTest
    {
        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            FileManager.CopyTestClasses();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            FileManager.DeleteTestClasses();
        }

        // Use TestInitialize to run code before running each test
        // [TestInitialize]
        // public void MyTestInitialize()
        // {
        // }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup]
        // public void MyTestCleanup()
        // {
        // }
        #endregion

        /// <summary>
        /// A test for the Constructor
        /// </summary>
        [TestMethod]
        public void Constructor()
        {
            var sut = new CodeFile("C:\\temp\\Program.cs", new int[] { -1, 0, 2 });

            Assert.AreEqual(2, sut.CoverableLines, "Not equal");
            Assert.AreEqual(1, sut.CoveredLines, "Not equal");
        }

        /// <summary>
        /// A test for Merge
        /// </summary>
        [TestMethod]
        public void Merge_MergeCodeFileWithEqualLengthCoverageArray_CoverageInformationIsUpdated()
        {
            var sut = new CodeFile("C:\\temp\\Program.cs", new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 });
            var codeFileToMerge = new CodeFile("C:\\temp\\Program.cs", new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 });

            sut.Merge(codeFileToMerge);

            Assert.AreEqual(8, sut.CoverableLines, "Not equal");
            Assert.AreEqual(5, sut.CoveredLines, "Not equal");
        }

        /// <summary>
        /// A test for Merge
        /// </summary>
        [TestMethod]
        public void Merge_MergeCodeFileWithLongerCoverageArray_CoverageInformationIsUpdated()
        {
            var sut = new CodeFile("C:\\temp\\Program.cs", new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 });
            var codeFileToMerge = new CodeFile("C:\\temp\\Program.cs", new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1 });

            sut.Merge(codeFileToMerge);

            Assert.AreEqual(10, sut.CoverableLines, "Not equal");
            Assert.AreEqual(6, sut.CoveredLines, "Not equal");
        }

        /// <summary>
        /// A test for AnalyzeFile
        /// </summary>
        [TestMethod]
        public void AnalyzeFile_ExistingFile_AnalysisIsReturned()
        {
            var sut = new CodeFile("C:\\temp\\Program.cs", new int[] { -2, -1, 0, 1 });

            Assert.IsNull(sut.TotalLines);

            var fileAnalysis = sut.AnalyzeFile();

            Assert.IsNotNull(fileAnalysis);
            Assert.IsNull(fileAnalysis.Error);
            Assert.AreEqual(fileAnalysis.Path, fileAnalysis.Path);
            Assert.AreEqual(26, sut.TotalLines);
            Assert.AreEqual(26, fileAnalysis.Lines.Count());

            Assert.AreEqual(1, fileAnalysis.Lines.ElementAt(0).LineNumber);
            Assert.AreEqual(-1, fileAnalysis.Lines.ElementAt(0).LineVisits);
            Assert.AreEqual(LineVisitStatus.NotCoverable, fileAnalysis.Lines.ElementAt(0).LineVisitStatus);

            Assert.AreEqual(2, fileAnalysis.Lines.ElementAt(1).LineNumber);
            Assert.AreEqual(0, fileAnalysis.Lines.ElementAt(1).LineVisits);
            Assert.AreEqual(LineVisitStatus.NotCovered, fileAnalysis.Lines.ElementAt(1).LineVisitStatus);

            Assert.AreEqual(3, fileAnalysis.Lines.ElementAt(2).LineNumber);
            Assert.AreEqual(1, fileAnalysis.Lines.ElementAt(2).LineVisits);
            Assert.AreEqual(LineVisitStatus.Covered, fileAnalysis.Lines.ElementAt(2).LineVisitStatus);
        }

        /// <summary>
        /// A test for AnalyzeFile
        /// </summary>
        [TestMethod]
        public void AnalyzeFile_NonExistingFile_AnalysisIsReturned()
        {
            var sut = new CodeFile("C:\\temp\\Other.cs", new int[] { -2, -1, 0, 1 });

            Assert.IsNull(sut.TotalLines);

            var fileAnalysis = sut.AnalyzeFile();

            Assert.IsNotNull(fileAnalysis);
            Assert.IsNotNull(fileAnalysis.Error);
            Assert.AreEqual(fileAnalysis.Path, fileAnalysis.Path);
            Assert.IsNull(sut.TotalLines);
            Assert.AreEqual(0, fileAnalysis.Lines.Count());
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals()
        {
            var target1 = new CodeFile("C:\\temp\\Program.cs", new int[0]);
            var target2 = new CodeFile("C:\\temp\\Program.cs", new int[0]);
            var target3 = new CodeFile("C:\\temp\\Other.cs", new int[0]);

            Assert.IsTrue(target1.Equals(target2), "Objects are not equal");
            Assert.IsFalse(target1.Equals(target3), "Objects are equal");
            Assert.IsFalse(target1.Equals(null), "Objects are equal");
            Assert.IsFalse(target1.Equals(new object()), "Objects are equal");
        }
    }
}
