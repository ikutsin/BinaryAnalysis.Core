using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using ExpressionSerialization;
using ExpressionSerialization.DLinq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NUnit.Framework;

namespace BinaryAnalysis.Tests.Data.BoxModule
{
    [TestFixture]
    public class RemoteTableTest
    {
        [Test]
        public void RemoteTableExpressionTest()
        {
            var rt = new RemoteTable<RTItem>();
            var query = rt.Where(x => x.name == "dd");

            Expression<Func<int, bool>> expression = x => (-x > 5) ? true : false;

            var el = rt.SerializeQuery();
        }
        public class RTItem
        {
            public string name { get; set; }
        }

        public class RemoteTable<T> : IQueryable<T>, IQueryProvider, IQueryService, IOrderedQueryable, IOrderedQueryable<T>
        {
            Expression expression;
            public Expression Expression { get { return expression; } }
            public IQueryProvider Provider { get { return this; } }
            public Type ElementType { get { return typeof(T); } }

            public RemoteTable()
            {
                expression = Expression.Constant(this);
            }

            private RemoteTable(Expression expression)
            {
                this.expression = expression;
            }

            public IEnumerator<T> GetEnumerator()
            {
                Object o = this.Execute(this.expression);
                IEnumerable enumerable = (IEnumerable)o;
                return enumerable.Cast<T>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new RemoteTable<TElement>(expression);
            }

            public IQueryable CreateQuery(Expression expression)
            {
                Type elementType = TypeSystem.GetElementType(expression.Type);
                Type queryType = typeof(IQueryable<>).MakeGenericType(new Type[] { elementType });

                return (IQueryable)Activator.CreateInstance(typeof(RemoteTable<>).MakeGenericType(new Type[] { elementType }), new object[] { expression });
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return (TResult)this.Execute(expression);
            }

            public object Execute(Expression expression)
            {
                XElement queryXml = this.SerializeQuery();
                Type elementType = TypeSystem.GetElementType(expression.Type);
                Type ienumerableExpressionType = TypeSystem.FindIEnumerable(expression.Type);
                return null;
            }

            internal static class TypeSystem
            {
                internal static Type GetElementType(Type seqType)
                {
                    Type type = FindIEnumerable(seqType);
                    if (type == null)
                    {
                        return seqType;
                    }
                    return type.GetGenericArguments()[0];
                }

                internal static Type FindIEnumerable(Type seqType)
                {
                    if ((seqType != null) && (seqType != typeof(string)))
                    {
                        if (seqType.IsArray)
                        {
                            return typeof(IEnumerable<>).MakeGenericType(new Type[] { seqType.GetElementType() });
                        }
                        if (seqType.IsGenericType)
                        {
                            foreach (Type type in seqType.GetGenericArguments())
                            {
                                Type type2 = typeof(IEnumerable<>).MakeGenericType(new Type[] { type });
                                if (type2.IsAssignableFrom(seqType))
                                {
                                    return type2;
                                }
                            }
                        }
                        Type[] interfaces = seqType.GetInterfaces();
                        if ((interfaces != null) && (interfaces.Length > 0))
                        {
                            foreach (Type type3 in interfaces)
                            {
                                Type type4 = FindIEnumerable(type3);
                                if (type4 != null)
                                {
                                    return type4;
                                }
                            }
                        }
                        if ((seqType.BaseType != null) && (seqType.BaseType != typeof(object)))
                        {
                            return FindIEnumerable(seqType.BaseType);
                        }
                    }
                    return null;
                }
            }
        }
    }
}
