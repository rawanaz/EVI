﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Slp.Evi.Storage.Database;
using Slp.Evi.Storage.Query;
using VDS.RDF;

namespace Slp.Evi.Storage.Relational.Query.ValueBinders
{
    /// <summary>
    /// The switch value binder
    /// </summary>
    public class SwitchValueBinder
        : IValueBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchValueBinder"/> class.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="caseVariable">The case variable.</param>
        /// <param name="cases">The cases.</param>
        public SwitchValueBinder(string variableName, ICalculusVariable caseVariable, IEnumerable<Case> cases)
        {
            VariableName = variableName;
            CaseVariable = caseVariable;
            Cases = cases;
        }

        /// <summary>
        /// Gets the cases.
        /// </summary>
        /// <value>The cases.</value>
        public IEnumerable<Case> Cases { get; }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <param name="data">The data.</param>
        /// <returns>The returned value from visitor.</returns>
        [DebuggerStepThrough]
        public object Accept(IValueBinderVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>
        /// Loads the node.
        /// </summary>
        /// <param name="nodeFactory">The node factory.</param>
        /// <param name="rowData">The row data.</param>
        /// <param name="context">The context.</param>
        public INode LoadNode(INodeFactory nodeFactory, IQueryResultRow rowData, IQueryContext context)
        {
            var columnName = context.QueryNamingHelpers.GetVariableName(null, CaseVariable);
            var column = rowData.GetColumn(columnName);

            if (column.GetIntegerValue().HasValue)
            {
                var value = column.GetIntegerValue().Value;

                var selectedCase = Cases.FirstOrDefault(x => x.CaseValue == value);

                if (selectedCase != null)
                {
                    return selectedCase.ValueBinder.LoadNode(nodeFactory, rowData, context);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the case variable.
        /// </summary>
        /// <value>The case variable.</value>
        public ICalculusVariable CaseVariable { get; }

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        /// <value>The name of the variable.</value>
        public string VariableName { get; }

        /// <summary>
        /// Gets the needed calculus variables to calculate the value.
        /// </summary>
        /// <value>The needed calculus variables.</value>
        public IEnumerable<ICalculusVariable> NeededCalculusVariables => ReferencedVariables.Distinct();

        /// <summary>
        /// Gets the needed calculus variables to calculate the value.
        /// </summary>
        /// <value>The needed calculus variables.</value>
        /// <remarks>May contain duplicate values</remarks>
        private IEnumerable<ICalculusVariable> ReferencedVariables
        {
            get
            {
                yield return CaseVariable;

                foreach (var neededCalculusVariable in Cases.SelectMany(@case => @case.ValueBinder.NeededCalculusVariables))
                {
                    yield return neededCalculusVariable;
                }
            }
        }

        /// <summary>
        /// Value binder case
        /// </summary>
        public class Case
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Case"/> class.
            /// </summary>
            /// <param name="caseValue">The case value.</param>
            /// <param name="valueBinder">The value binder.</param>
            public Case(int caseValue, IValueBinder valueBinder)
            {
                CaseValue = caseValue;
                ValueBinder = valueBinder;
            }

            /// <summary>
            /// Gets the case value.
            /// </summary>
            /// <value>The case value.</value>
            public int CaseValue { get; }

            /// <summary>
            /// Gets the value binder.
            /// </summary>
            /// <value>The value binder.</value>
            public IValueBinder ValueBinder { get; }
        }
    }
}
