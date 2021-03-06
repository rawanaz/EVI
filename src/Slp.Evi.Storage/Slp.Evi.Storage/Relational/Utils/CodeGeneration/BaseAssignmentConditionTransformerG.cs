﻿// This is generated code, do not edit!!!
using System;

using Slp.Evi.Storage.Relational.Query;
using Slp.Evi.Storage.Relational.Query.Conditions.Assignment;
namespace Slp.Evi.Storage.Relational.Utils.CodeGeneration
{
    /// <summary>
    /// Base generated transformer for <see cref="IAssignmentConditionVisitor" />
    /// </summary>
    /// <typeparam name="T">Type of parameter passed to process</typeparam>
    /// <typeparam name="TR">Type of the transformation result</typeparam>
    /// <typeparam name="T1">Type of the transformation result when processing <see cref="ISourceCondition" /></typeparam>
    /// <typeparam name="T2">Type of the transformation result when processing <see cref="IFilterCondition" /></typeparam>
    /// <typeparam name="T3">Type of the transformation result when processing <see cref="ICalculusSource" /></typeparam>
    public abstract class BaseAssignmentConditionTransformerG<T, TR, T1, T2, T3>
        : BaseSourceConditionTransformerG<T, T1, T2, T3>, IAssignmentConditionVisitor
    {
        /// <summary>
        /// Transforms the <see cref="IAssignmentCondition" />.
        /// </summary>
        /// <param name="instance">The instance to transform.</param>
        /// <param name="data">The passed data.</param>
        /// <returns>The transformed calculus source.</returns>
        public TR TransformAssignmentCondition(IAssignmentCondition instance, T data)
        {
            return (TR)instance.Accept(this, data);
        }
        /// <summary>
        /// Decides whether we should use standard or fallback transformation for the transformation.
        /// </summary>
        /// <param name="toTransform">Instance to be transformed</param>
        /// <param name="data">The passed data</param>
        /// <returns><c>true</c> if transformation should process standardly, <c>false</c> the fallback should be used.</returns>
        protected virtual bool CommonShouldTransform(IAssignmentCondition toTransform, T data)
        {
            return true;
        }

        /// <summary>
        /// Post-process for the transformation.
        /// </summary>
        /// <param name="transformed">The transformation result.</param>
        /// <param name="toTransform">The transformed instance</param>
        /// <param name="data">The passed data.</param>
        /// <returns>The post-processed transformation result</returns>
        protected virtual TR CommonPostTransform(TR transformed, IAssignmentCondition toTransform, T data)
        {
            return transformed;
        }

        /// <summary>
        /// Fallback variant for the transformation.
        /// </summary>
        /// <param name="toTransform">Instance to be transformed.</param>
        /// <param name="data">The passed data.</param>
        /// <returns>The transformation result</returns>
        protected virtual TR CommonFallbackTransform(IAssignmentCondition toTransform, T data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits <see cref="AssignmentFromExpressionCondition" />
        /// </summary>
        /// <param name="toVisit">The visited instance</param>
        /// <param name="data">The passed data</param>
        /// <returns>The returned data</returns>
        public object Visit(AssignmentFromExpressionCondition toVisit, object data)
        {
            return ProcessVisit(toVisit, (T)data);
        }

        /// <summary>
        /// Processes the visit of <see cref="AssignmentFromExpressionCondition" />
        /// </summary>
        /// <param name="toVisit">The visited instance</param>
        /// <param name="data">The passed data</param>
        /// <returns>The returned data</returns>
        protected virtual TR ProcessVisit(AssignmentFromExpressionCondition toVisit, T data) 
        {
            if(ShouldTransform(toVisit, data))
            {
                var transformed = Transform(toVisit, data);
                return PostTransform(transformed, toVisit, data);
            }
            else
            {
                return FallbackTransform(toVisit, data);
            }
        }

        /// <summary>
        /// Process the <see cref="AssignmentFromExpressionCondition"/>
        /// </summary>
        /// <param name="toTransform">The instance to process</param>
        /// <param name="data">The passed data</param>
        /// <returns>The transformation result</returns>
        protected abstract TR Transform(AssignmentFromExpressionCondition toTransform, T data);

        /// <summary>
        /// Pre-process for the transformation.
        /// </summary>
        /// <param name="toTransform">Instance to be transformed</param>
        /// <param name="data">The passed data</param>
        /// <returns><c>true</c> if transformation should continue, <c>false</c> the fallback should be used.</returns>
        protected virtual bool ShouldTransform(AssignmentFromExpressionCondition toTransform, T data)
        {
            return CommonShouldTransform(toTransform, data);
        }

        /// <summary>
        /// Post-process for the transformation.
        /// </summary>
        /// <param name="transformed">The transformation result.</param>
        /// <param name="toTransform">The transformed instance</param>
        /// <param name="data">The passed data.</param>
        /// <returns>The post-processed transformation result</returns>
        protected virtual TR PostTransform(TR transformed, AssignmentFromExpressionCondition toTransform, T data)
        {
            return CommonPostTransform(transformed, toTransform, data);
        }

        /// <summary>
        /// Fallback variant for the transformation.
        /// </summary>
        /// <param name="toTransform">Instance to be transformed.</param>
        /// <param name="data">The passed data.</param>
        /// <returns>The transformation result</returns>
        protected virtual TR FallbackTransform(AssignmentFromExpressionCondition toTransform, T data)
        {
            return CommonFallbackTransform(toTransform, data);
        }

    }
}
