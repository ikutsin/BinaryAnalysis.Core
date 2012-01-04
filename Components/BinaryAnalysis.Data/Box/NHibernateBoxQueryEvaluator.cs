using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using Autofac;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Metrics;
using BinaryAnalysis.Data.Settings;
using ExpressionSerialization;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Box;
using System.Collections;
using log4net;
using NHibernate.Linq;
using NHibernate;

namespace BinaryAnalysis.Data.Box
{
    public delegate IDbContext ContextProvider();
    public class NHibernateBoxQueryEvaluator
    {
        public ContextProvider ContextProvider { get; set; }

        public RepositoryFinder RepoFinder
        {
            get { return _repoFinder; }
        }

        ILog log;
        private readonly RepositoryFinder _repoFinder;
        private readonly IComponentContext context;

        public NHibernateBoxQueryEvaluator(ILog log, 
            RepositoryFinder repoFinder, IComponentContext context)
        {
            this.log = log;
            _repoFinder = repoFinder;
            this.context = context;
        }

        public object Evaluate(XElement evaluatorXml)
        {
            XElement queryXml = evaluatorXml.Element("QueryXml");
            Type elementType = 
                ExpressionSerializer.ParseTypeOrNullFromXml(evaluatorXml.Element("elementType"));
            Type ienumerableExpressionType =
                ExpressionSerializer.ParseTypeOrNullFromXml(evaluatorXml.Element("ienumerableExpressionType"));
            var exprType = DeserializeExpression(queryXml.Elements().First());

            return Evaluate(exprType, elementType, ienumerableExpressionType);
        }

        public object Evaluate(System.Tuple<Expression, Type> exprType, Type elementType, Type ienumerableExpressionType)
        {
            var repoInfo = RepoFinder.CreateRepository(exprType.Item2, ComponentsLoadLevel.Always);
            object returnObject = null;

            using (repoInfo.Item1)
            {
                //TODO: call it with reduced rights
                var queryResult = repoInfo.Item2.GetMethod("ExecuteDetachedExpression")
                    .Invoke(repoInfo.Item1, new object[] { exprType.Item1, elementType, ienumerableExpressionType });

                
                var attr = (BoxToAttribute)exprType.Item2.GetCustomAttributes(typeof(BoxToAttribute), false).FirstOrDefault();
                if (queryResult != null)
                {
                    if (attr.BoxingType == elementType)
                    {
                        var transformerType = typeof(Box.NHibernateBoxTransformation<,>)
                            .MakeGenericType(elementType, exprType.Item2);
                        var transformer = context.Resolve(transformerType);

                        if (ienumerableExpressionType == null)
                        {
                            var qrl = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(exprType.Item2));
                            qrl.Add(queryResult);

                            transformerType.GetProperty("Entries").SetValue(transformer, qrl, null);
                            var box = (IBox)transformerType.GetMethod("ToBox").Invoke(transformer, null) as IBox;
                            returnObject = box.Count > 0 ? box[0] : null;
                        }
                        else
                        {
                            transformerType.GetProperty("Entries").SetValue(transformer, queryResult, null);
                            var box = transformerType.GetMethod("ToBox").Invoke(transformer, null) as IBox;
                            returnObject = box;
                        }

                    }
                    else
                    {
                        returnObject = queryResult;
                    }
                }
            }
            return returnObject;
        }

        protected System.Tuple<Expression, Type> DeserializeExpression(XElement rootXml)
        {
            //move to ctor
            BoxSerializationTypeResolver resolver = new BoxSerializationTypeResolver(this);
            BoxCustomExpressionXmlConverter customConverter = new BoxCustomExpressionXmlConverter(resolver, RepoFinder.Mappings);
            ExpressionSerializer serializer = new ExpressionSerializer(resolver) { Converters = { customConverter } };

            return new System.Tuple<Expression, Type>(serializer.Deserialize(rootXml), customConverter.QueryKind.ElementType);
        }

        private class BoxSerializationTypeResolver : ExpressionSerializationTypeResolver
        {
            NHibernateBoxQueryEvaluator evaler;
            public BoxSerializationTypeResolver(NHibernateBoxQueryEvaluator evaler)
            {
                this.evaler = evaler;
            }

            protected override Type ResolveTypeFromString(string typeString)
            {
                if (typeString.Contains('`'))
                    return null;
                if (typeString.Contains(','))
                    typeString.Substring(0, typeString.IndexOf(','));

                foreach (var kvp in evaler.RepoFinder.Mappings)
                {
                    if (typeString.EndsWith(kvp.Item2.Name))
                        return kvp.Item1;
                    if (typeString.EndsWith(kvp.Item2.Name + "[]"))
                        return typeof(Box<>).MakeGenericType(kvp.Item1);
                }
                return null;
            }
        }

        private class BoxCustomExpressionXmlConverter : CustomExpressionXmlConverter
        {
            private ExpressionSerializationTypeResolver resolver;
            private readonly List<System.Tuple<Type, Type>> mappings;
            //private ISession session;
            public IQueryable QueryKind { get; private set; }

            public BoxCustomExpressionXmlConverter(
                ExpressionSerializationTypeResolver resolver, List<System.Tuple<Type,Type>> mappings)
            {
                //this.session = session;
                this.resolver = resolver;
                this.mappings = mappings;
            }

            public override Expression Deserialize(XElement expressionXml)
            {
                if (expressionXml.Name.LocalName == "Table")
                {
                    //ITable table = dc.GetTable(type); // REturning a random IQueryable of the right kind so that we can re-create the IQueryable // instance at the end of this method...
                    //TODO: It is maybe better directly create the kind from Nhibernate.Linq?
                    // In that case - the session has to be controlled in Evaluator class
                    Type type = resolver.GetType(expressionXml.Attribute("Type").Value);
                    //Type type = mappings.First(t => t.Item2 == typeb).Item1;

                    Type ecg = typeof(NhQueryable<>).MakeGenericType(type);
                    QueryKind = (IQueryable)Activator.CreateInstance(ecg, new object[] { null });
                    return Expression.Constant(QueryKind);
                }
                return null;
            }
            public override XElement Serialize(Expression expression)
            {
                throw new NotImplementedException();
            }
        }

        /*
        protected IQueryable DeserializeQuery(XElement rootXml)
        {
            Expression queryExpr = serializer.Deserialize(rootXml);
            // Query kind is populated by the ResolveXmlFromExpression method - Deserialize
            if (customConverter.QueryKind == null)
                throw new Exception(string.Format("Mapping type not found"));

            //Type ecg = typeof(ExpressionContainer<>).MakeGenericType(customConverter.QueryKind.ElementType);
            //ecg.GetProperty("Expression").SetValue(customConverter.QueryKind, queryExpr, null);
            //return customConverter.QueryKind;
            
            return customConverter.QueryKind.Provider.CreateQuery(queryExpr);
        }
         */
    }
}
