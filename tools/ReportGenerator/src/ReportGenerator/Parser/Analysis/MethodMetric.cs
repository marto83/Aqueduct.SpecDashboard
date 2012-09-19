﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Palmmedia.ReportGenerator.Parser.Analysis
{
    /// <summary>
    /// Represents the metrics of a method.
    /// </summary>
    public class MethodMetric
    {
        /// <summary>
        /// List of metrics.
        /// </summary>
        private readonly List<Metric> metrics = new List<Metric>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodMetric"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MethodMetric(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodMetric"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="metrics">The metrics.</param>
        public MethodMetric(string name, IEnumerable<Metric> metrics)
        {
            this.Name = name;
            this.AddMetrics(metrics);
        }

        /// <summary>
        /// Gets the list of metrics.
        /// </summary>
        public IEnumerable<Metric> Metrics
        {
            get
            {
                return this.metrics;
            }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Adds the given metric.
        /// </summary>
        /// <param name="metric">The metric.</param>
        public void AddMetric(Metric metric)
        {
            this.metrics.Add(metric);
        }

        /// <summary>
        /// Adds the given metrics.
        /// </summary>
        /// <param name="metrics">The metrics to add.</param>
        public void AddMetrics(IEnumerable<Metric> metrics)
        {
            this.metrics.AddRange(metrics);
        }

        /// <summary>
        /// Merges the given method metric with the current instance.
        /// </summary>
        /// <param name="methodMetric">The method metric to merge.</param>
        public void Merge(MethodMetric methodMetric)
        {
            if (methodMetric == null)
            {
                throw new ArgumentNullException("methodMetric");
            }

            foreach (var metric in methodMetric.metrics)
            {
                var existingMetric = this.metrics.FirstOrDefault(m => m.Name == metric.Name);
                if (existingMetric != null)
                {
                    existingMetric.Value = Math.Max(existingMetric.Value, metric.Value);
                }
                else
                {
                    this.AddMetric(metric);
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
            if (obj == null || !obj.GetType().Equals(typeof(MethodMetric)))
            {
                return false;
            }
            else
            {
                var methodMetric = (MethodMetric)obj;
                return methodMetric.Name.Equals(this.Name);
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
