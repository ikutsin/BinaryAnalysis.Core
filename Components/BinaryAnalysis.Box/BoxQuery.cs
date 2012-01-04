using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Xml.Linq;
using BinaryAnalysis.Box;
using ExpressionSerialization;

namespace BinaryAnalysis.Data.Box
{
    public static class BoxedQueryRemoteExtensions
    {
        //Query provider is called without CTOR :o //Oleg Usanov - 19.03.11
        public static Func<XElement, object> DefaultEvaluator { get; set; }

        public static XElement SerializeQuery(this IQueryable query)
        {
            return BoxedQueryRemoteExtensions.SerializeQuery(query.Expression);
        }
        public static XElement SerializeQuery(this Expression expression)
        {
            ExpressionSerializer serializer = new ExpressionSerializer() { Converters = { new BoxedExpressionXmlConverter() } };
            return serializer.Serialize(expression);
        }
        private class BoxedExpressionXmlConverter : CustomExpressionXmlConverter
        {
            public override XElement Serialize(Expression expression)
            {
                if (typeof(IQueryProvider).IsAssignableFrom(expression.Type))
                {
                    return new XElement("Table", new XAttribute("Type", expression.Type.GetGenericArguments()[0].FullName));
                }
                return null;
            }

            public override Expression Deserialize(XElement expressionXml)
            {
                throw new NotImplementedException();
            }
        }
    }
    public class BoxQuery<TM> : IQueryable<TM>, IQueryProvider, IOrderedQueryable, IOrderedQueryable<TM>
    {
        Expression expression;
        public Expression Expression { get { return expression; } }
        public IQueryProvider Provider { get { return this; } }
        public Type ElementType { get { return typeof(TM); } }

        public XElement Serialize()
        {
            return BoxedQueryRemoteExtensions.SerializeQuery(this);
        }

        public BoxQuery()
        {
            expression = Expression.Constant(this);
        }

        public BoxQuery(Expression expression)
        {
            this.expression = expression;
        }

        public IEnumerator<TM> GetEnumerator()
        {
            Object o = this.Execute(this.expression);
            IEnumerable enumerable = (IEnumerable)o;
            return enumerable.Cast<TM>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new BoxQuery<TElement>(expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            Type queryType = typeof(IQueryable<>).MakeGenericType(new Type[] { elementType });

            var thisType = typeof(BoxQuery<>).MakeGenericType(new Type[] { elementType });
            return (IQueryable)Activator.CreateInstance(thisType, new object[] { expression });
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this.Execute(expression);
        }

        public object Execute(Expression expression)
        {
            if (BoxedQueryRemoteExtensions.DefaultEvaluator == null) throw new InvalidOperationException("BoxedQueryRemoteExtensions.DefaultEvaluator is not set");
            XElement queryXml = BoxedQueryRemoteExtensions.SerializeQuery(expression);
            Type elementType = TypeSystem.GetElementType(expression.Type);
            Type ienumerableExpressionType = TypeSystem.FindIEnumerable(expression.Type);
            XElement evaluatorXml = new XElement("evaluatorXml",
                new XElement("QueryXml", queryXml),
                ExpressionSerializer.GenerateXmlFromTypeOrNull("elementType", elementType),
                ExpressionSerializer.GenerateXmlFromTypeOrNull("ienumerableExpressionType", ienumerableExpressionType));
            return BoxedQueryRemoteExtensions.DefaultEvaluator(evaluatorXml);
        }


    }
    public static class TypeSystem
    {
        public static Type GetElementType(Type seqType)
        {
            Type type = FindIEnumerable(seqType);
            if (type == null)
            {
                return seqType;
            }
            return type.GetGenericArguments()[0];
        }

        public static Type FindIEnumerable(Type seqType)
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
