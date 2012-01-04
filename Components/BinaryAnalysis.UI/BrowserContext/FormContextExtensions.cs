using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Autofac;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.UI.Commons.Data;
using BinaryAnalysis.Data.Core;
using Newtonsoft.Json;

namespace BinaryAnalysis.UI.BrowserContext
{
    [ComVisible(true)]
    public class FormContextExtensions : IBrowserContextExtension
    {
        private readonly RepositoryFinder _repoFinder;
        private readonly IComponentContext _componentContext;

        public FormContextExtensions(RepositoryFinder repoFinder,
            IComponentContext componentContext)
        {
            _repoFinder = repoFinder;
            _componentContext = componentContext;
        }

        public bool validateEntity(string data, string typename)
        {
            var box = GetPostDataBoxTransformationResult(data, typename)[0];

            ValidationContext context = new ValidationContext(box, null, null);
            List<ValidationResult> results = new List<ValidationResult>();
            return Validator.TryValidateObject(box, context, results, true);
        }
        public void deleteEntity(int id, string typename)
        {
            var bType = Type.GetType(typename);
            var eType = _repoFinder.Mappings.First(t => t.Item2 == bType).Item1;

            var rt = _repoFinder.CreateRepository(eType);
            var repoType = rt.Item2;
            var repo = rt.Item1;

            var e = repoType.GetMethod("Get").Invoke(repo, new object[] {id, Enums.LockMode.None});
            repoType.GetMethod("Delete").Invoke(repo, new[] {e});
        }
        public string getEntity(int id, string typename)
        {
            var bType = Type.GetType(typename);
            var eType = _repoFinder.Mappings.First(t => t.Item2 == bType).Item1;

            var rt = _repoFinder.CreateRepository(eType);
            var repoType = rt.Item2;
            var repo = rt.Item1;

            var dataTransType = typeof(NHibernateBoxTransformation<,>).MakeGenericType(bType, eType);
            var dataTrans = _componentContext.Resolve(dataTransType);

            var qrl = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(eType));
            qrl.Add(repoType.GetMethod("Get").Invoke(repo, new object[] { id, Enums.LockMode.None}));

            dataTransType.GetProperty("Entries").SetValue(dataTrans, qrl, null);
            var box = (IBox)dataTransType.GetMethod("ToBox").Invoke(dataTrans, null) as IBox;

            var transType = typeof(PostDataBoxTransformation<>).MakeGenericType(bType);
            var trans = _componentContext.Resolve(transType);
            transType.GetMethod("Transform").Invoke(trans, new[] { box });
            var results = transType.GetProperty("Results").GetValue(trans, null) as string[];

            return results.FirstOrDefault();
        }
        public string getEntityBox(int id, string typename)
        {
            var bType = Type.GetType(typename);
            var eType = _repoFinder.Mappings.First(t => t.Item2 == bType).Item1;

            var rt = _repoFinder.CreateRepository(eType);
            var repoType = rt.Item2;
            var repo = rt.Item1;

            var dataTransType = typeof(NHibernateBoxTransformation<,>).MakeGenericType(bType, eType);
            var dataTrans = _componentContext.Resolve(dataTransType);

            var qrl = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(eType));
            qrl.Add(repoType.GetMethod("Get").Invoke(repo, new object[] { id, Enums.LockMode.None }));

            dataTransType.GetProperty("Entries").SetValue(dataTrans, qrl, null);
            var box = (IBox)dataTransType.GetMethod("ToBox").Invoke(dataTrans, null) as IBox;
            
            if (box.Count == 0) return null;
            return JsonConvert.SerializeObject(box[0]);
        }

        public void saveEntity(string data, string typename)
        {
            var box = GetPostDataBoxTransformationResult(data, typename);
            SetNHibernateTransformation(box, typename);
        }

        private IBox GetPostDataBoxTransformationResult(string data, string typename)
        {
            var type = Type.GetType(typename);
            var transType = typeof(PostDataBoxTransformation<>).MakeGenericType(type);
            var trans = _componentContext.Resolve(transType);
            transType.GetProperty("Results").SetValue(trans, new[] { data }, null);
            var box = transType.GetMethod("ToBox").Invoke(trans, null) as IBox;
            return box;
        }
        private void SetNHibernateTransformation(IBox box, string typename)
        {
            var bType = Type.GetType(typename);
            var eType = _repoFinder.Mappings.First(t => t.Item2 == bType).Item1;

            var dataTransType = typeof(NHibernateBoxTransformation<,>).MakeGenericType(bType, eType);
            var dataTrans = _componentContext.Resolve(dataTransType);
            dataTransType.GetProperty("ImportStrategy").SetValue(dataTrans, BoxImporterStrategy.UpdateExisting, null);
            dataTransType.GetMethod("Transform").Invoke(dataTrans, new[] { box });
        }
    }
}
