using System.Linq.Expressions;

namespace Aco228.Common.Helpers;

public static class ExpressionsHelper
{
    public static Expression<Func<T, bool>> CombineFilters<T>(
        IEnumerable<Expression<Func<T, bool>>> filters)
    {
        if (!filters.Any())
            return x => true;

        var param = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        foreach (var filter in filters)
        {
            // Replace the parameter in the filter with our single parameter
            var visitor = new ParameterReplaceVisitor(filter.Parameters[0], param);
            var visitedBody = visitor.Visit(filter.Body)!;

            body = body == null ? visitedBody : Expression.AndAlso(body, visitedBody);
        }

        return Expression.Lambda<Func<T, bool>>(body!, param);
    }
    
    
    class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam;
        private readonly ParameterExpression _newParam;

        public ParameterReplaceVisitor(ParameterExpression oldParam, ParameterExpression newParam)
        {
            _oldParam = oldParam;
            _newParam = newParam;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParam ? _newParam : base.VisitParameter(node);
        }
    }
}
