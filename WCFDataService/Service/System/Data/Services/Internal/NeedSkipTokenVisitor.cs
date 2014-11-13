//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Internal
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Visitor to evaluate if skip tokens are needed for a given <see cref="OrderingInfo"/>
    /// </summary>
    internal sealed class NeedSkipTokenVisitor : ALinqExpressionVisitor
    {
        /// <summary>Resource type for which we are evaluating ordering expressions</summary>
        private readonly ResourceType rt;

        /// <summary>Resource property to which the ordering expression corresponds</summary>
        private ResourceProperty property;

        /// <summary>Initializes a new <see cref="NeedSkipTokenVisitor"/> instance.</summary>
        private NeedSkipTokenVisitor()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="NeedSkipTokenVisitor"/> instance.
        /// </summary>
        /// <param name="rt">Resource type for which we are evaluating ordering expressions</param>
        private NeedSkipTokenVisitor(ResourceType rt)
        {
            this.rt = rt;
        }

        /// <summary>
        /// True of skiptoken is required for this instance, false otherwise
        /// </summary>
        private bool NeedSkipToken
        {
            get;
            set;
        }

        /// <summary>
        /// Resource property to which the ordering expression corresponds
        /// </summary>
        private ResourceProperty Property
        {
            get
            {
                return this.rt != null ? this.property : null;
            }
        }

        /// <summary>
        /// Finds out if the given <paramref name="orderingInfo"/> required a skip token 
        /// expression in the expansion
        /// </summary>
        /// <param name="orderingInfo">Input orderingInfo.</param>
        /// <returns>true if skip token expression is needed, false otherwise</returns>
        internal static bool IsSkipTokenRequired(OrderingInfo orderingInfo)
        {
            if (orderingInfo != null && orderingInfo.IsPaged)
            {
                foreach (OrderingExpression o in orderingInfo.OrderingExpressions)
                {
                    LambdaExpression l = (LambdaExpression)o.Expression;
                    NeedSkipTokenVisitor visitor = new NeedSkipTokenVisitor();
                    visitor.Visit(l.Body);
                    if (visitor.NeedSkipToken)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Obtains a collection of resource properties that are needed for skip token generation
        /// </summary>
        /// <param name="orderingInfo">Input orderingInfo.</param>
        /// <param name="rt">Resource type for which to collect the skip token properties</param>
        /// <returns>Collection of resource properties used in $skiptoken</returns>
        internal static ICollection<ResourceProperty> CollectSkipTokenProperties(OrderingInfo orderingInfo, ResourceType rt)
        {
            Debug.Assert(orderingInfo != null, "Must have valid ordering information to collect skip token properties");
            Debug.Assert(orderingInfo.IsPaged, "Must have paging enabled to collection skip token properties");

            List<ResourceProperty> resourceProperties = new List<ResourceProperty>();
            foreach (OrderingExpression o in orderingInfo.OrderingExpressions)
            {
                LambdaExpression l = (LambdaExpression)o.Expression;
                NeedSkipTokenVisitor visitor = new NeedSkipTokenVisitor(rt);
                visitor.Visit(l.Body);
                if (visitor.NeedSkipToken)
                {
                    return null;
                }

                Debug.Assert(visitor.Property != null, "Must have a valid property if skip token is not needed");
                resourceProperties.Add(visitor.Property);
            }

            return resourceProperties;
        }

        /// <summary>
        /// Override the <see cref="ExpressionVisitor"/> method to decide if we need skip token expression in the expansion
        /// </summary>
        /// <param name="exp">Input expression</param>
        /// <returns>Output expression which is the same as input expression for this visitor</returns>
        internal override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return exp;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.MemberAccess:
                case ExpressionType.Parameter:
                    return base.Visit(exp);
                default:
                    this.NeedSkipToken = true;
                    return exp;
            }
        }

        /// <summary>
        /// Override for member access visitor
        /// </summary>
        /// <param name="m">Member access expression</param>
        /// <returns>Same expressions as <paramref name="m"/></returns>
        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.MemberType != MemberTypes.Property || m.Expression.NodeType != ExpressionType.Parameter)
            {
                this.NeedSkipToken = true;
                return m;
            }

            // Is this a visitor for collection of resource properties
            if (this.rt != null)
            {
                this.property = this.rt.TryResolvePropertyName(m.Member.Name, exceptKind: ResourcePropertyKind.Stream);
                Debug.Assert(
                    this.property != null && this.property.ResourceType.ResourceTypeKind == ResourceTypeKind.Primitive,
                    "skip token should be made up of primitive properties.");
            }

            return base.VisitMemberAccess(m);
        }

        /// <summary>
        /// Override for parameter expression
        /// </summary>
        /// <param name="p">Parameter expression</param>
        /// <returns>Same parameter as <paramref name="p"/></returns>
        internal override Expression VisitParameter(ParameterExpression p)
        {
            if (this.rt != null && p.Type != this.rt.InstanceType)
            {
                this.NeedSkipToken = true;
                return p;
            }

            return base.VisitParameter(p);
        }
    }
}
