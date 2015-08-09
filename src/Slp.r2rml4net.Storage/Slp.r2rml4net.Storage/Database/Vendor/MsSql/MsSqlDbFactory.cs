﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slp.r2rml4net.Storage.Bootstrap;
using Slp.r2rml4net.Storage.Database.Base;

namespace Slp.r2rml4net.Storage.Database.Vendor.MsSql
{
    /// <summary>
    /// Sql factory for MS SQL
    /// </summary>
    public class MsSqlDbFactory 
        : ISqlDbFactory
    {
        /// <summary>
        /// Creates the SQL database connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public virtual ISqlDatabase CreateSqlDb(string connectionString)
        {
            return new MsSqlDb(this, connectionString);
        }

        /// <summary>
        /// Creates the SQL query builder.
        /// </summary>
        public virtual ISqlQueryBuilder CreateSqlQueryBuilder()
        {
            return new BaseSqlQueryBuilder();
        }
    }
}