using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Cfg.Loquacious;
using VEnv = NHibernate.Validator.Cfg.Environment;
using FluentNHibernate.Cfg.Db;
using FHN = FluentNHibernate.Cfg;
using System.Reflection;
using FluentNHibernate.Conventions;
using BinaryAnalysis.Data.Core.Conventions;
using BinaryAnalysis.Data.Core.SessionManagement;
using NHibernate.Search.Event;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using NHibernate.Search.Store;
using NHibernate.ByteCode.Castle;
using NHibernate.Event;

namespace BinaryAnalysis.Data.Core.Impl
{
    //http://marcinobel.com/index.php/fluent-nhibernate-conventions-examples/
    //http://wiki.fluentnhibernate.org/Conventions

    public class ModuledDbContext : IDbContext
    {
        protected ISessionFactory sessionFactory;
        protected ISessionManager sessionManager;
        protected Configuration currentConfiguration;
        protected ValidatorEngine validationEngine;

        public IEnumerable<IConvention> MappingConventions { get; set; }
        public IEnumerable<ITypedListener> Listeners { get; set; }
        public IEnumerable<INHPropertiesProvider> PropertyProviders { get; set; }
        public IEnumerable<INHMappingsProvider> MappingProviders { get; set; }

        public void Initialize(ISessionManager sessionManager) //, IPersistenceConfigurer persistenceConfigurer)
        {
            sessionManager.Init(this);
            this.sessionManager = sessionManager;

            currentConfiguration = new Configuration();
            currentConfiguration.Configure();

            var fc = FHN.Fluently.Configure(currentConfiguration)
                //.Database(persistenceConfigurer)
                //.Diagnostics(x=>x.OutputToConsole())
                .Mappings(x=> { 
                    foreach(var prov in  MappingProviders) {
                        foreach(var mapping in prov.FluentMappings) {
                            x.FluentMappings.Add(mapping);
                        }
                    }
                    x.FluentMappings.Conventions.Add(MappingConventions.ToArray());
                });

            currentConfiguration = fc.BuildConfiguration();

            InitializeValidation();
            InitListeners();

            currentConfiguration.SetProperty("proxyfactory.factory_class", typeof(ProxyFactoryFactory).AssemblyQualifiedName);

            sessionFactory = currentConfiguration.BuildSessionFactory();
        }

        protected virtual void InitListeners()
        {
            Dictionary<ListenerType, List<ITypedListener>> listenerDict = new Dictionary<ListenerType,List<ITypedListener>>();
            foreach (var l in Listeners)
            {
                foreach (var lt in l.ListenerTypes)
                {
                    if (!listenerDict.ContainsKey(lt)) listenerDict.Add(lt, new List<ITypedListener>());
                    listenerDict[lt].Add(l);
                }
            }
            foreach (var kvp in listenerDict)
            {
                Type listenerTypeI = NHEventListeners.EventInterfaceFromType[kvp.Key];
                MethodInfo mCast = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(listenerTypeI);
                MethodInfo mToArray = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(listenerTypeI);

                var larr = (object[])mToArray.Invoke(null, new object[] {mCast.Invoke(null, new object[] { kvp.Value})});
                currentConfiguration.AppendListeners(kvp.Key, larr);
            }

            foreach (var pp in PropertyProviders)
            {
                foreach (var ppp in pp.Properties)
                {
                    currentConfiguration.SetProperty(ppp.Key, ppp.Value);
                }
            }
        }

        protected virtual void InitializeValidation()
        {
            validationEngine = new ValidatorEngine();
            var configuration = new FluentConfiguration();

            foreach (var prov in MappingProviders)
            {
                configuration.Register(prov.FluentMappings.ValidationDefinitions());
            } 
            configuration.SetDefaultValidatorMode(ValidatorMode.UseAttribute);

            validationEngine.Configure(configuration);
            currentConfiguration.Initialize(validationEngine);
        }

        public ValidatorEngine ValidatorEngine 
        { 
            get { return validationEngine; } 
        }

        public SchemaExport GetSchemaExport()
        {
            return new SchemaExport(currentConfiguration);
        }

        public void ValidateSchema()
        {
            var validator = new SchemaValidator(currentConfiguration);
            validator.Validate();
        }

        public ISessionFactory SessionFactory
        {
            get { return sessionFactory; }
        }

        public Configuration CurrentConfiguration
        {
            get { return currentConfiguration; }
        }

        public virtual ISessionManager SessionManager
        {
            get { return sessionManager; }
        }
    }
}
