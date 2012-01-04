using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.UI.Commons.Data;
using Newtonsoft.Json;

namespace BinaryAnalysis.UI.BrowserContext
{
    [ComVisible(true)]
    public class TreeContextExtension : IBrowserContextExtension
    {
        private readonly TaxonomyTree _tree;
        private readonly RelationRepository _relationRepo;
        private NHibernateBoxTransformation<RelationBoxMap, RelationEntity> relationTransformer;

        dynamic ToJson(TaxonomyNode node)
        {
            return new 
                       {
                           id = node.Path,
                           text = node.Name,
                           description = node.Description,
                           expanded = false,
                           hasChildren = node.Children.Count
                       };
        }

        public TreeContextExtension(TaxonomyTree tree, RelationRepository relationRepo,
            NHibernateBoxTransformation<RelationBoxMap, RelationEntity> relationTransformer)
        {
            _tree = tree;
            _relationRepo = relationRepo;
            this.relationTransformer = relationTransformer;
        }

        public string getTaxon(string path)
        {
            var tree = _tree.FindOne(path);
            return JsonConvert.SerializeObject(ToJson(_tree.FindOne(path)));
        }
        public string getTaxonChildren(string path)
        {
            var children = JsonConvert.SerializeObject(_tree.FindOne(path).Children.Select(ToJson));
            return children;
        }
        public string getRelationsByType(string path)
        {
            var taxNode = _tree.FindOne(path);
            if (taxNode == null) return null;

            relationTransformer.Entries = _relationRepo.GetRelationsByType(taxNode);
            return JsonConvert.SerializeObject(relationTransformer.ToBox());
        }
        public string getClassified(string path)
        {
            var taxNode = _tree.FindOne(path);
            if (taxNode == null) return null;

            relationTransformer.Entries = _relationRepo.GetAllRelationsFor(taxNode);

            return JsonConvert.SerializeObject(relationTransformer.ToBox());
        }

    }
}