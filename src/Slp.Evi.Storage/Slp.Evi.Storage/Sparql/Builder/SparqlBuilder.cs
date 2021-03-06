﻿using System;
using System.Collections.Generic;
using System.Linq;
using Slp.Evi.Storage.Common.Algebra;
using Slp.Evi.Storage.Query;
using Slp.Evi.Storage.Sparql.Algebra;
using Slp.Evi.Storage.Sparql.Algebra.Expressions;
using Slp.Evi.Storage.Sparql.Algebra.Modifiers;
using Slp.Evi.Storage.Sparql.Algebra.Patterns;
using VDS.RDF.Query;
using VDS.RDF.Query.Algebra;
using VDS.RDF.Query.Expressions.Comparison;
using VDS.RDF.Query.Expressions.Conditional;
using VDS.RDF.Query.Expressions.Functions.Sparql.Boolean;
using VDS.RDF.Query.Expressions.Primary;
using VDS.RDF.Query.Patterns;
using FilterPattern = Slp.Evi.Storage.Sparql.Algebra.Patterns.FilterPattern;
using ISparqlExpression = VDS.RDF.Query.Expressions.ISparqlExpression;
using Slp.Evi.Storage.Utils;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Ordering;

namespace Slp.Evi.Storage.Sparql.Builder
{
    /// <summary>
    /// SPARQL algebra builder.
    /// </summary>
    public class SparqlBuilder
    {
        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <param name="context">The query context.</param>
        /// <returns>The SPARQL query.</returns>
        /// <exception cref="System.Exception">Cannot handle unknown query type</exception>
        public ISparqlQuery Process(IQueryContext context)
        {
            switch (context.OriginalQuery.QueryType)
            {
                case SparqlQueryType.Ask:
                    return ProcessAsk(context);
                case SparqlQueryType.Construct:
                    return ProcessConstruct(context);
                case SparqlQueryType.Describe:
                case SparqlQueryType.DescribeAll:
                    return ProcessDescribe(context);
                case SparqlQueryType.Select:
                case SparqlQueryType.SelectAll:
                case SparqlQueryType.SelectAllDistinct:
                case SparqlQueryType.SelectAllReduced:
                case SparqlQueryType.SelectDistinct:
                case SparqlQueryType.SelectReduced:
                    return ProcessSelect(context);
                default:
                    throw new Exception("Cannot handle unknown query type");
            }
        }

        /// <summary>
        /// Processes the ask query.
        /// </summary>
        /// <param name="context">The query context.</param>
        /// <returns>The SPARQL query.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private ISparqlQuery ProcessAsk(IQueryContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes the construct query.
        /// </summary>
        /// <param name="context">The query context.</param>
        /// <returns>The SPARQL query.</returns>
        private ISparqlQuery ProcessConstruct(IQueryContext context)
        {
            return ProcessAlgebra(context.OriginalAlgebra, context);
        }

        /// <summary>
        /// Processes the describe query.
        /// </summary>
        /// <param name="context">The query context.</param>
        /// <returns>The SPARQL query.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private ISparqlQuery ProcessDescribe(IQueryContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes the select query.
        /// </summary>
        /// <param name="context">The query context.</param>
        /// <returns>The SPARQL query.</returns>
        private ISparqlQuery ProcessSelect(IQueryContext context)
        {
            return ProcessAlgebra(context.OriginalAlgebra, context);
        }

        /// <summary>
        /// Processes the algebra.
        /// </summary>
        /// <param name="originalAlgebra">The original algebra.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The SPARQL query.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private ISparqlQuery ProcessAlgebra(ISparqlAlgebra originalAlgebra, IQueryContext context)
        {
            if (originalAlgebra is Select orSel)
            {
                var innerAlgebra = ProcessAlgebra(orSel.InnerAlgebra, context);

                if (!orSel.IsSelectAll)
                {
                    return new SelectModifier(innerAlgebra, orSel.SparqlVariables.Select(x => x.Name).ToList());
                }
                else
                {
                    return new SelectModifier(innerAlgebra, innerAlgebra.Variables);
                }
            }
            else if (originalAlgebra is IBgp orBgp)
            {
                return ProcessTriplePatterns(orBgp.TriplePatterns);
            }
            else if (originalAlgebra is Union orUnion)
            {
                var left = (IGraphPattern)ProcessAlgebra(orUnion.Lhs, context);
                var right = (IGraphPattern)ProcessAlgebra(orUnion.Rhs, context);
                return new UnionPattern(new IGraphPattern[] { left, right });
            }
            else if (originalAlgebra is LeftJoin leftJoin)
            {
                var left = (IGraphPattern)ProcessAlgebra(leftJoin.Lhs, context);
                var right = (IGraphPattern)ProcessAlgebra(leftJoin.Rhs, context);
                var condition = ProcessCondition(leftJoin.Filter.Expression, context);

                return new LeftJoinPattern(left, right, condition);
            }
            else if (originalAlgebra is Filter filter)
            {
                var inner = (IGraphPattern) ProcessAlgebra(filter.InnerAlgebra, context);
                var innerExpression = ProcessCondition(filter.SparqlFilter.Expression, context);

                return new FilterPattern(inner, innerExpression);
            }
            else if (originalAlgebra is Join join)
            {
                var left = (IGraphPattern) ProcessAlgebra(join.Lhs, context);
                var right = (IGraphPattern) ProcessAlgebra(join.Rhs, context);

                return new JoinPattern(new IGraphPattern[] {left, right});
            }
            else if (originalAlgebra is Extend extend)
            {
                var inner = (IGraphPattern) ProcessAlgebra(extend.InnerAlgebra, context);

                var expression = ProcessExpression(extend.AssignExpression, context);

                return new ExtendPattern(inner, extend.VariableName, expression);
            }
            else if (originalAlgebra is OrderBy orderBy)
            {
                var inner = ProcessAlgebra(orderBy.InnerAlgebra, context);

                return CreateOrderBy(inner, orderBy.Ordering, context);
            }
            else if (originalAlgebra is Slice slice)
            {
                var inner = ProcessAlgebra(slice.InnerAlgebra, context);

                int? limit = (slice.Limit != -1) ? (int?)slice.Limit : null;
                int? offset = (slice.Offset != 0) ? (int?) slice.Offset : null;

                return new SliceModifier(inner, inner.Variables, limit, offset);
            }

            throw new NotImplementedException();

            // http://www.dotnetrdf.org/api/~VDS.RDF.Query.Algebra.html
            //Ask
            //AskAnyTriples
            //AskBgp
            //AskUnion
            //BaseArbitraryLengthPathOperator
            //BaseMultiset
            //BasePathOperator
            //BaseSet
            //Bgp
            //Bindings
            //Distinct
            //ExistsJoin
            //Extend
            //Filter
            //FilteredProduct
            //FullTextQuery
            //Graph
            //GroupBy
            //GroupMultiset
            //Having
            //IdentityFilter
            //IdentityMultiset
            //Join
            //LazyBgp
            //LazyUnion
            //LeftJoin
            //Minus
            //Multiset
            //NegatedPropertySet
            //NullMultiset
            //NullOperator
            //OneOrMorePath
            //OrderBy
            //ParallelJoin
            //ParallelUnion
            //PartitionedMultiset
            //PropertyFunction
            //PropertyPath
            //Reduced
            //SameTermFilter
            //Select
            //SelectDistinctGraphs
            //Service
            //Set
            //SetDistinctnessComparer
            //SingleValueRestrictionFilter
            //Slice
            //SubQuery
            //Table
            //Union
            //VariableRestrictionFilter
            //ZeroLengthPath
            //ZeroOrMorePath

        }

        private OrderByModifier CreateOrderBy(ISparqlQuery inner, ISparqlOrderBy ordering, IQueryContext context)
        {
            List<OrderByModifier.OrderingPart> parts = new List<OrderByModifier.OrderingPart>();

            var current = ordering;

            while (current != null)
            {
                if (!current.IsSimple)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    var variable = ((VariableExpression) ProcessExpression(current.Expression, context)).Variable;
                    parts.Add(new OrderByModifier.OrderingPart(variable, current.Descending));
                }

                current = current.Child;
            }

            return new OrderByModifier(inner, inner.Variables, parts);
        }

        private ISparqlCondition ProcessCondition(ISparqlExpression expression, IQueryContext context)
        {
            var processed = ProcessExpression(expression, context);

            if (processed is ISparqlCondition sparqlCondition)
            {
                return sparqlCondition;
            }
            else if (processed is NodeExpression nodeExpression)
            {
                var node = nodeExpression.Node;

                if (node.EffectiveType == XmlSpecsHelper.XmlSchemaDataTypeBoolean)
                {
                    if (node.AsBoolean())
                    {
                        return new BooleanTrueExpression();
                    }
                    else
                    {
                        return new BooleanFalseExpression();
                    }
                }
            }

            throw new ArgumentException("The expression needs to be convertible to condition", nameof(expression));
        }

        private Algebra.ISparqlExpression ProcessExpression(ISparqlExpression expression, IQueryContext context)
        {
            if (expression is BoundFunction boundFunction)
            {
                return new IsBoundExpression(boundFunction.Variables.Single());
            }
            else if (expression is AndExpression conjunction)
            {
                var left = ProcessCondition(conjunction.Arguments.ElementAt(0), context);
                var right = ProcessCondition(conjunction.Arguments.ElementAt(1), context);
                return new ConjunctionExpression(new ISparqlCondition[] {left, right});
            }
            else if (expression is OrExpression disjunction)
            {
                var left = ProcessCondition(disjunction.Arguments.ElementAt(0), context);
                var right = ProcessCondition(disjunction.Arguments.ElementAt(1), context);
                return new DisjunctionExpression(new ISparqlCondition[] { left, right });
            }
            else if (expression is NotExpression notExpression)
            {
                return new NegationExpression(ProcessCondition(notExpression.Arguments.Single(), context));
            }
            else if (expression is GreaterThanExpression greaterThan)
            {
                var left = ProcessExpression(greaterThan.Arguments.ElementAt(0), context);
                var right = ProcessExpression(greaterThan.Arguments.ElementAt(1), context);
                return new ComparisonExpression(left, right, ComparisonTypes.GreaterThan);
            }
            else if (expression is GreaterThanOrEqualToExpression greaterThanOrEqual)
            {
                var left = ProcessExpression(greaterThanOrEqual.Arguments.ElementAt(0), context);
                var right = ProcessExpression(greaterThanOrEqual.Arguments.ElementAt(1), context);
                return new ComparisonExpression(left, right, ComparisonTypes.GreaterOrEqualThan);
            }
            else if (expression is LessThanExpression lessThan)
            {
                var left = ProcessExpression(lessThan.Arguments.ElementAt(0), context);
                var right = ProcessExpression(lessThan.Arguments.ElementAt(1), context);
                return new ComparisonExpression(left, right, ComparisonTypes.LessThan);
            }
            else if (expression is LessThanOrEqualToExpression lessThanOrEqual)
            {
                var left = ProcessExpression(lessThanOrEqual.Arguments.ElementAt(0), context);
                var right = ProcessExpression(lessThanOrEqual.Arguments.ElementAt(1), context);
                return new ComparisonExpression(left, right, ComparisonTypes.LessOrEqualThan);
            }
            else if (expression is EqualsExpression equals)
            {
                var left = ProcessExpression(equals.Arguments.ElementAt(0), context);
                var right = ProcessExpression(equals.Arguments.ElementAt(1), context);
                return new ComparisonExpression(left, right, ComparisonTypes.EqualTo);
            }
            else if (expression is NotEqualsExpression notEquals)
            {
                var left = ProcessExpression(notEquals.Arguments.ElementAt(0), context);
                var right = ProcessExpression(notEquals.Arguments.ElementAt(1), context);
                return new ComparisonExpression(left, right, ComparisonTypes.NotEqualTo);
            }
            else if (expression is VariableTerm variableTerm)
            {
                var variable = variableTerm.Variables.Single();
                return new VariableExpression(variable);
            }
            else if (expression is ConstantTerm constantTerm)
            {
                return new NodeExpression(constantTerm.Node());
            }

            throw new NotImplementedException();
        }

        private IGraphPattern ProcessTriplePatterns(IEnumerable<ITriplePattern> triplePatterns)
        {
            List<IGraphPattern> joinedQueries = new List<IGraphPattern>();

            foreach (var triplePattern in triplePatterns.OfType<VDS.RDF.Query.Patterns.TriplePattern>())
            {
                var processed = new Algebra.Patterns.TriplePattern(triplePattern.Subject, triplePattern.Predicate,
                    triplePattern.Object);
                joinedQueries.Add(processed);
            }

            IGraphPattern currentQuery;
            if (joinedQueries.Count == 0)
            {
                currentQuery = new EmptyPattern();
            }
            else if (joinedQueries.Count == 1)
            {
                currentQuery = joinedQueries[0];
            }
            else
            {
                currentQuery = new JoinPattern(joinedQueries);
            }

            // TODO:
            //    http://www.dotnetrdf.org/api/dotNetRDF~VDS.RDF.Query.Patterns.ITriplePattern.html
            //    //BindPattern
            //    //FilterPattern
            //    //LetPattern
            //    //PropertyFunctionPattern
            //    //PropertyPathPattern
            //    //SubQueryPattern
            //    //TriplePattern

            return currentQuery;
        }
    }
}
