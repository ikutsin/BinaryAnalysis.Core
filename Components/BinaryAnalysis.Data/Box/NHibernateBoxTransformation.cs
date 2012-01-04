using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Box;
using BinaryAnalysis.Box.Transforations;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Metrics;
using BinaryAnalysis.Data.Settings;
using log4net;
using BinaryAnalysis.Data.Core.Impl;
using EmitMapper.MappingConfiguration;
using EmitMapper;


namespace BinaryAnalysis.Data.Box
{
	public enum BoxImporterStrategy
	{
		ErrorExisting, SkipExisting, UpdateExisting, RewriteExisting
	}

	//TODO: remove entity dependency (find it in mappings)
	public class NHibernateBoxTransformation<TM, TE> : IBaseBoxTransformation<TM>
		where TM : EntityBoxMap
		where TE : Entity
	{
		private readonly IDbContext _context;
		internal readonly RepositoryFinder _repoFinder;
		private readonly ILog _log;
		private readonly TaxonomyTree _tree;

	    public NHibernateBoxTransformation(IDbContext context, RepositoryFinder repoFinder, ILog log,
	                                       TaxonomyTree tree)
	        : this(context, repoFinder, log, tree, BoxImporterStrategy.ErrorExisting)
	    {
	    }

	    public NHibernateBoxTransformation(IDbContext context, RepositoryFinder repoFinder, ILog log,
			TaxonomyTree tree, BoxImporterStrategy strategy)
		{
			_context = context;
			_repoFinder = repoFinder;
			_log = log;
			_tree = tree;

		    ImportStrategy = strategy;
			FindExistingEntity = new Func<IRepository<TE>, TE, TE>((r, e) => r.Get(e.Id));
		}

	    private IEnumerable<TE> entries;
        public IEnumerable<TE> Entries { get { return entries; } set { entries = value.Where(x => x != null); } }

		#region Implementation of IBaseBoxTransformation<T>

		public IBox<TM> ToBox()
		{
            if (Entries == null) throw new InvalidOperationException("Entries has not been set");

			EnsureTypesAreValid();
            var converter = new NHibernateBoxConverter<TE, TM>(_repoFinder);
            var box = converter.ToBox(Entries);
			//custom boxing
			for (int i = 0; i < box.Count(); i++)
			{
                CustomBoxing(Entries.Skip(i).First(), box.Skip(i).First());
			}
			return box;
		}

		public void Transform(IBox<TM> box)
		{
			EnsureTypesAreValid();
			var converter = new NHibernateBoxConverter<TE, TM>(_repoFinder);
			var entries = converter.ToEntity(box);
			var entriez = new List<TE>();
			
			//persist (set tu null which to skip)
			var repo = _repoFinder.CreateRepository<TE>();
			foreach (var entry in entries)
			{
				var e = FindExistingEntity.Invoke(repo, entry);
				if (e != null)
				{
                    switch (ImportStrategy)
                    {
                        case BoxImporterStrategy.ErrorExisting:
                            throw new Exception("Error strategy fired");
                            break;
                        case BoxImporterStrategy.SkipExisting:
                            _log.Debug(e.Id + " skipped");
                            entriez.Add(null);
                            break;
                        case BoxImporterStrategy.UpdateExisting:
                            var mapper = ObjectMapperManager.DefaultInstance
                                .GetMapper<TE, TE>(new DefaultMapConfig()
                                .IgnoreMembers<TE, TE>(new[] { "Id" }));

                            mapper.Map(entry, e);
                            var entryx = repo.Update(e);
                            _log.Debug(entryx.Id + " updated");
                            entriez.Add(entryx);
                            break;
                        case BoxImporterStrategy.RewriteExisting:
					        repo.Delete(e);
                            entry.Id = 0;
                            var entryy = repo.Save(entry);
                            _log.Debug(entryy.Id + " rewritten");
                            entriez.Add(entryy);
                            break;
                        default:
                            throw new Exception("Unknown import strategy");
                            break;
                    }
				}
				else
				{
					entry.Id = 0;
                    var entryy = repo.Save(entry);
                    entriez.Add(entryy);
				}
			}
            if (box.Count() != entriez.Count) throw new InvalidOperationException("Entyty vs Box count mismatch");

		    var result = new List<TE>();
            //custom unboxing
			for (int i = 0; i < box.Count(); i++)
			{
                if (entriez[i] != null)
                {
                    CustomUnboxing(entriez[i], box.Skip(i).First());
                    result.Add(entriez[i]);
                }
                else
                {
                    result.Add(entries[i]);
                }
			}
		    Entries = result;
		}
		#endregion

		#region System

		public BoxImporterStrategy ImportStrategy { get; set; }
		public Func<IRepository<TE>, TE, TE> FindExistingEntity { get; set; }

		void EnsureTypesAreValid()
		{
			var attrs = typeof(TE).GetCustomAttributes(typeof(BoxToAttribute), false).Cast<BoxToAttribute>();
			if (attrs.Count() == 0) throw new Exception("This class can not be boxed because BoxToAttribute is NOT set in class");
			if (!attrs.Any(a => a.BoxingType == typeof(TM))) throw new Exception("BoxToAttribute not found for specified entity/box type");
		}

		void CustomBoxing(object e, object eMap)
		{
			//IList<Classifications>
			var classifiacationProp = eMap.GetType().GetProperty("Classifications");
			if (classifiacationProp != null && e is IClassifiable)
			{
				var nodes = _tree.GetClassifications(e as IClassifiable);
				classifiacationProp.SetValue(eMap, nodes.Select(n => n.Path).ToList(), null);
			}

            //Box<> - convert, don't persist
            var emapEntriesProp = eMap.GetType().GetProperty("Entries");
            if (emapEntriesProp != null && typeof(IBox).IsAssignableFrom(emapEntriesProp.PropertyType))
            {
                var eEntriesProp = e.GetType().GetProperty("Entries");
                if (eEntriesProp != null)
                {
                    var contverterType = typeof(NHibernateBoxConverter<,>)
                        .MakeGenericType(
                            eEntriesProp.PropertyType.GetGenericArguments()[0],
                            emapEntriesProp.PropertyType.GetGenericArguments()[0]
                            );
                    var converter = Activator.CreateInstance(contverterType, _repoFinder);
                    var box = contverterType.GetMethods().Where(m=>m.Name=="ToBox").Skip(1).First()
                        .Invoke(converter, new [] {eEntriesProp.GetValue(e, null)});
                    emapEntriesProp.SetValue(eMap, box, null);

                    //var transformerType = typeof(NHibernateBoxTransformation<,>)
                    //    .MakeGenericType(
                    //        emapEntriesProp.PropertyType.GetGenericArguments()[0],
                    //        eEntriesProp.PropertyType.GetGenericArguments()[0]);
                    //var transformer = Activator.CreateInstance(transformerType, _context, _repoFinder, _log, _tree, BoxImporterStrategy.RewriteExisting);
                    //transformerType.GetProperty("Entries").SetValue(transformer, eEntriesProp.GetValue(e, null), null);
                    //var box = transformerType.GetMethod("ToBox").Invoke(transformer, null) as IBox;
                    //emapEntriesProp.SetValue(eMap, box, null);
                }
            }

			//SettingsBoxMap
			var settingsProp = eMap.GetType().GetProperty("Settings");
			if (settingsProp != null && e.GetType().GetProperty("Settings") == null && e is IClassifiable)
			{
				var settings = _repoFinder.SettingsService.GetFor(e as IClassifiable);
                var sConverter = CreateNestedTransformer<SettingsBoxMap, SettingsEntity>(BoxImporterStrategy.UpdateExisting);
                sConverter.Entries = new [] { settings };
				settingsProp.SetValue(eMap, sConverter.ToBox().First(), null);
			}

			//IBox<MetricsBoxMap>
			var metricsProp = eMap.GetType().GetProperty("Metrics");
			if (metricsProp != null && e.GetType().GetProperty("Metrics") == null && e is IClassifiable)
			{
				var metrics = _repoFinder.MetricsService.GetFor(e as IClassifiable);
                var mConverter = CreateNestedTransformer<MetricsBoxMap, MetricsEntity>(BoxImporterStrategy.UpdateExisting);
			    mConverter.Entries = metrics;
				metricsProp.SetValue(eMap, mConverter.ToBox(), null);
			}
		}
		void CustomUnboxing(object e, object eMap)
		{
			//IList<Classifications>
			var classifiacationProp = eMap.GetType().GetProperty("Classifications");
			if (classifiacationProp != null && e is IClassifiable)
			{
				var ec = e as IClassifiable;
				var classifications = (IList<string>)classifiacationProp.GetValue(eMap, null);
				foreach (string classification in classifications)
				{
					_tree.Classify(ec, _tree.GetOrCreatePath(classification));
				}
			}

            //Box<>
            var emapEntriesProp = eMap.GetType().GetProperty("Entries");
            if (emapEntriesProp != null && typeof(IBox).IsAssignableFrom(emapEntriesProp.PropertyType))
            {
                var eEntriesProp = e.GetType().GetProperty("Entries");
                if (eEntriesProp != null)
                {
                    var contverterType = typeof(NHibernateBoxConverter<,>)
                        .MakeGenericType(
                            eEntriesProp.PropertyType.GetGenericArguments()[0],
                            emapEntriesProp.PropertyType.GetGenericArguments()[0]
                            );
                    var converter = Activator.CreateInstance(contverterType, _repoFinder);
                    var box = contverterType.GetMethods().Where(m => m.Name == "ToEntity").Skip(1).First()
                        .Invoke(converter, new[] { emapEntriesProp.GetValue(eMap, null) });
                    eEntriesProp.SetValue(e, box, null);

                    //var transformerType = typeof(NHibernateBoxTransformation<,>)
                    //    .MakeGenericType(
                    //        emapEntriesProp.PropertyType.GetGenericArguments()[0],
                    //        eEntriesProp.PropertyType.GetGenericArguments()[0]);
                    //var transformer = Activator.CreateInstance(transformerType, _context, _repoFinder, _log, _tree, BoxImporterStrategy.RewriteExisting);

                    //transformerType.GetMethod("Transform").Invoke(transformer, new[] { emapEntriesProp.GetValue(eMap, null) });
                    //eEntriesProp.SetValue(e, transformerType.GetProperty("Entries").GetValue(transformer, null), null);
                }
            }

			//SettingsBoxMap
			var settingsProp = eMap.GetType().GetProperty("Settings");
			if (settingsProp != null && e.GetType().GetProperty("Settings") == null && e is IClassifiable)
			{
                var sConverter = CreateNestedTransformer<SettingsBoxMap, SettingsEntity>(BoxImporterStrategy.UpdateExisting);
                var settingsBox = (SettingsBoxMap)settingsProp.GetValue(eMap, null);
                sConverter.Transform(new Box<SettingsBoxMap>(settingsBox));
			    var settings = sConverter.Entries.First();
                foreach (var entry in settings.Entries) entry.Settings = settings;
                _repoFinder.SettingsService.SaveFor((e as IClassifiable), settings);
			}

			//IBox<MetricsBoxMap>
			var metricsProp = eMap.GetType().GetProperty("Metrics");
			if (metricsProp != null && e.GetType().GetProperty("Metrics") == null && e is IClassifiable)
			{
                var mConverter = CreateNestedTransformer<MetricsBoxMap, MetricsEntity>(BoxImporterStrategy.UpdateExisting);
				var metricsBox = ((IBox<MetricsBoxMap>)metricsProp.GetValue(eMap, null));
			    mConverter.Transform(metricsBox);
			    var metrics = mConverter.Entries;
                foreach (var m in metrics) foreach (var me in m.Entries) me.Metrics = m;
                mConverter.Entries.ToList().ForEach(m => _repoFinder.MetricsService.EnsureRelation((e as IClassifiable), m));
                _repoFinder.MetricsService.UpdateMetrics(mConverter.Entries.ToList());
			}

		}
		#endregion

        NHibernateBoxTransformation<G, GE> CreateNestedTransformer<G, GE>(BoxImporterStrategy strat)
            where G : EntityBoxMap
            where GE : Entity
        {
            var transformer = new NHibernateBoxTransformation<G, GE>(_context,_repoFinder,_log,_tree);
            transformer.ImportStrategy = strat;
            return transformer;
        }
	}
}
