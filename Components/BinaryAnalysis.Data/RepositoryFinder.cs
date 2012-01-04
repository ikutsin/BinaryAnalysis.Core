using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Metrics;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Data.Versioning;
using log4net;

namespace BinaryAnalysis.Data.Box
{
    public class RepositoryFinder
    {
        private readonly IDbContext dbContext;
        private readonly ILog log;
        public SettingsService SettingsService { get; set; }
        public RelationService RelationRepository { get; set; }
        public TrackingRepository TrackingRepository { get; set; }
        public MetricsService MetricsService { get; set; }

        public RepositoryFinder(IDbContext dbContext, ILog log)
        {
            this.dbContext = dbContext;
            this.log = log;
        }

        public Tuple<IDisposable, Type> CreateRepository(Type entity, ComponentsLoadLevel loadLevel = ComponentsLoadLevel.Single)
        {
            //XXX: Type resolving crutch
            Type repoType = null;
            IDisposable repo = null;

            if (entity.GetInterfaces().Contains(typeof(ISettingsHolder)))
            {
                repoType = typeof(SettingsHolderRepository<>).MakeGenericType(entity);
                repo = (IDisposable)Activator.CreateInstance(repoType, new object[]
                {
                    dbContext, log, SettingsService, RelationRepository
                });
            }
            else if (entity.GetInterfaces().Contains(typeof(IMetricsHolder)))
            {
                repoType = typeof(MetricsHolderRepository<>).MakeGenericType(entity);
                repo = (IDisposable)Activator.CreateInstance(repoType, new object[]
                {
                    dbContext, MetricsService, log, RelationRepository
                });
            }
            else if (entity.GetInterfaces().Contains(typeof(ITrackable)))
            {
                repoType = typeof(TrackableRepository<>).MakeGenericType(entity);
                repo = (IDisposable)Activator.CreateInstance(repoType, new object[]
                {
                    dbContext, log, RelationRepository, TrackingRepository
                });
            }
            else if (entity.GetInterfaces().Contains(typeof(IClassifiable)))
            {
                repoType = typeof(ClassifiableRepository<>).MakeGenericType(entity);
                repo = (IDisposable)Activator.CreateInstance(repoType, new object[]
                {
                    dbContext, log, RelationRepository
                });
            }
            else
            {
                repoType = typeof(Repository<>).MakeGenericType(entity);
                repo = (IDisposable)Activator.CreateInstance(repoType, new object[] 
                { 
                    dbContext, log 
                });
            }

            repoType.GetProperty("ComponentsLoadLevel").SetValue(repo, loadLevel, null);
            return new Tuple<IDisposable, Type>(repo, repoType);
        }

        public IRepository<T> CreateRepository<T>() where T : class
        {
            return (IRepository<T>)CreateRepository(typeof (T)).Item1;
        }

        List<Tuple<Type, Type>> _mappings;
        /// <summary>
        /// tuple<Entityclass, BoxClass>
        /// </summary>
        public List<Tuple<Type, Type>> Mappings
        {
            get
            {
                if (_mappings == null)
                {
                    _mappings = (from c in dbContext.CurrentConfiguration.ClassMappings.Select(x => x.MappedClass)
                                 from g in c.GetCustomAttributes(typeof (BoxToAttribute), false).Cast<BoxToAttribute>()
                                 select new Tuple<Type, Type>(c, g.BoxingType)).ToList();
                }
                return _mappings;
            }
        }
    }
}
