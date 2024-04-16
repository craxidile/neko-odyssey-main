using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SpatiumInteractive.Libraries.Unity.Platform.Expressions
{
    public class SelectList<TSource>
    {
        private List<MemberInfo> members = new List<MemberInfo>();
        public SelectList<TSource> Add<TValue>(Expression<Func<TSource, TValue>> selector)
        {
            var member = ((MemberExpression)selector.Body).Member;
            members.Add(member);
            return this;
        }
        public IQueryable<TResult> Select<TResult>(IQueryable<TSource> source)
        {
            var sourceType = typeof(TSource);
            var resultType = typeof(TResult);
            var parameter = Expression.Parameter(sourceType, "e");
            var bindings = members.Select(member => Expression.Bind(
                resultType.GetProperty(member.Name), Expression.MakeMemberAccess(parameter, member)));
            var body = Expression.MemberInit(Expression.New(resultType), bindings);
            var selector = Expression.Lambda<Func<TSource, TResult>>(body, parameter);
            return source.Select(selector);
        }
    }
}
