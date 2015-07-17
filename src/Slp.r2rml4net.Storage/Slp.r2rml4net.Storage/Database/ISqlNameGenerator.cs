﻿using Slp.r2rml4net.Storage.Query;
using Slp.r2rml4net.Storage.Relational.Query.Source;

namespace Slp.r2rml4net.Storage.Database
{
    /// <summary>
    /// Sql names generator
    /// </summary>
    public interface ISqlNameGenerator
    {
        /// <summary>
        /// Generates the names.
        /// </summary>
        /// <param name="calculusModel">The calculus model.</param>
        /// <param name="context">The context.</param>
        void GenerateNames(CalculusModel calculusModel, QueryContext context);
    }
}