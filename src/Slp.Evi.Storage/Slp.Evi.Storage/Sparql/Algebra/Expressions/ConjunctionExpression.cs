﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Slp.Evi.Storage.Sparql.Algebra.Expressions
{
    /// <summary>
    /// Representation of AND expression
    /// </summary>
    public class ConjunctionExpression 
        : ISparqlCondition
    {
        /// <summary>
        /// Gets the operands.
        /// </summary>
        /// <value>The operands.</value>
        public ISparqlCondition[] Operands { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConjunctionExpression"/> class.
        /// </summary>
        /// <param name="operands">The operands.</param>
        public ConjunctionExpression(IEnumerable<ISparqlCondition> operands)
        {
            Operands = operands.ToArray();
            NeededVariables = Operands.SelectMany(x => x.NeededVariables).Distinct().ToArray();
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <param name="data">The data.</param>
        /// <returns>The returned value from visitor.</returns>
        [DebuggerStepThrough]
        public object Accept(ISparqlExpressionVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>
        /// Gets the needed variables to evaluate the expression.
        /// </summary>
        public IEnumerable<string> NeededVariables { get; }
    }
}
