using System;
using System.Linq.Expressions;

namespace SSD
{
    public static class PropertyReflection
    {
        public static string GetMemberName<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return ((MemberExpression)expression.Body).Member.Name;
        }
    }
}
