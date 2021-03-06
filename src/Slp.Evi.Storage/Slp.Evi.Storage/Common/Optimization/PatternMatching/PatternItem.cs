﻿using System;
using Slp.Evi.Storage.Relational.Query;
using Slp.Evi.Storage.Relational.Query.ValueBinders;

namespace Slp.Evi.Storage.Common.Optimization.PatternMatching
{
    /// <summary>
    /// Class part of pattern in the pattern matching.
    /// </summary>
    public class PatternItem
    {
        /// <summary>
        /// Value indicating whether this instance is column.
        /// </summary>
        private readonly bool _isColumn;

        /// <summary>
        /// Gets a value indicating whether this instance is column.
        /// </summary>
        /// <value><c>true</c> if this instance is column; otherwise, <c>false</c>.</value>
        public bool IsColumn => _isColumn;
        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value><c>true</c> if this instance is constant; otherwise, <c>false</c>.</value>
        public bool IsConstant => !_isColumn;
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternItem"/> class of column type.
        /// </summary>
        public PatternItem()
        {
            _isColumn = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternItem"/> class of constant type with
        /// <paramref name="text"/> as text.
        /// </summary>
        public PatternItem(string text)
        {
            _isColumn = false;
            Text = text;
        }

        /// <summary>
        /// Gets the assigned text (if the instance is constant)
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the relational column of this pattern item.
        /// </summary>
        public ICalculusVariable RelationalColumn { get; set; }

        /// <summary>
        /// Creates <see cref="PatternItem"/> from the template part.
        /// </summary>
        /// <param name="templatePart">The template part.</param>
        public static PatternItem FromTemplatePart(ITemplatePart templatePart)
        {
            if (templatePart.IsText)
            {
                return new PatternItem(templatePart.Text);
            }
            else if (templatePart.IsColumn)
            {
                return new PatternItem();
            }
            else
            {
                throw new InvalidOperationException("ITemplatePart has to be text or column");
            }
        }
    }
}
