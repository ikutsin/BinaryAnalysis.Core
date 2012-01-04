using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using System.Text.RegularExpressions;
using BinaryAnalysis.Data.Core.Impl;
using log4net;

namespace BinaryAnalysis.Data
{
    public class TaxonomyTree
    {
        public static readonly string[] SEPARATORS = new string[] { "/", "\\" };

        TaxonRepository repo;
        RelationService relationSrv;
        internal ILog log;

        int rootId;
        TaxonEntity rootEntity;
        TaxonomyNode rootNode;
        public TaxonomyTree(ILog log, TaxonRepository repo, RelationService relationSrv):this(log,repo, relationSrv, 1)
        {
        }
        public TaxonomyTree(ILog log, TaxonRepository repo, RelationService relationSrv, int rootId)
        {
            this.log = log;
            this.rootId = rootId;
            this.repo = repo;
            this.relationSrv = relationSrv;
            rootEntity = repo.Get(rootId);

            if (rootEntity == null)
            {
                if (rootId == 1)
                {
                    rootEntity = new TaxonEntity()
                    {
                        Name = "Root",
                        Description = "Root node - parent of all taxon entries"
                    };
                    repo.Save(rootEntity);
                }
                else
                {
                    throw new Exception("Root node is not found");
                }
            }        
            rootNode = new TaxonomyNode(this, rootEntity);
        }

        public TaxonomyNode Root { get { return rootNode; } }
        public TaxonomyNode GetOrCreatePath(string query, string defaultDescription = null)
        {
            var queryArr = query.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
            var currentNode = Root;
            string nextName = "";
            for (var i = 0; i < queryArr.Length; i++)
            {
                nextName = queryArr[i];
                if (currentNode.HasChild(nextName)) currentNode = currentNode[nextName];
                else currentNode = currentNode.AddChild(nextName, i == queryArr.Length-1?defaultDescription:null);
            }
            return currentNode;
        }
        public TaxonomyNode FindOne(string query)
        {
            var queryArr = query.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
            var currentNode = Root;
            string nextName = "";
            for (var i = 0; i < queryArr.Length; i++)
            {
                nextName = queryArr[i];
                if (currentNode.HasChild(nextName)) currentNode = currentNode[nextName];
                else return null;
            }
            return currentNode;
        }
        public IList<TaxonomyNode> Find(string query)
        {
            var queryArr = query.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => "^" + Regex.Escape(x).Replace("\\*", ".*").Replace("\\?", ".") + "$").ToArray();

            var lastMatches = new List<TaxonomyNode>() { rootNode };
            for (int i = 0; i < queryArr.Length; i++)
            {
                var newMatches = new List<TaxonomyNode>();
                var regex = new Regex(queryArr[i]);
                foreach (var lastNode in lastMatches)
                {
                    newMatches.AddRange(lastNode.Children.Where(x => regex.IsMatch(x.Name) && !x.IsDetached));
                }
                lastMatches = newMatches;
            }
            return lastMatches;
        }

        #region node callbacks
        internal TaxonomyNode LoadParent(TaxonomyNode taxon)
        {
            if (taxon.IsDetached) throw new Exception(String.Format("Taxon '{0}' is detached", taxon.Name));
            var entity = relationSrv.GetByRelated<TaxonEntity>(taxon.entity, RelationDirection.Forward, rootNode).FirstOrDefault();
            if (entity == null) return null;
            return new TaxonomyNode(this, entity);
        }
        internal List<TaxonomyNode> LoadChildren(TaxonomyNode taxon)
        {
            if (taxon.IsDetached) throw new Exception(String.Format("Taxon '{0}' is detached", taxon.Name));
            var children = new List<TaxonomyNode>();
            var entities = relationSrv.GetRelated<TaxonEntity>(taxon.entity, RelationDirection.Forward, rootNode).ToList();
            foreach (var entity in entities)
            {
                var newNode = new TaxonomyNode(this, entity) { parent = taxon };
                children.Add(newNode);
            }
            return children;
        }

        #region Classification
        public void Classify(IClassifiable classifiable, TaxonomyNode node)
        {
            relationSrv.AddRelation(classifiable, node, RelationDirection.Both);
        }

        public bool IsClassified(IClassifiable classifiable, TaxonomyNode node)
        {
            return relationSrv.IsRelated(classifiable, node);
        }

        public void Declassify(IClassifiable classifiable, TaxonomyNode node)
        {
            relationSrv.RemoveRelation(classifiable, node);
        }

        public IList<T> GetClassifiables<T>(TaxonomyNode node) where T : Entity, IClassifiable
        {
            return relationSrv.GetRelated<T>(node, RelationDirection.Both);
        }
        public IList<TaxonomyNode> GetClassifications(IClassifiable classifiable)
        {
            return relationSrv.GetRelated<TaxonEntity>(classifiable, RelationDirection.Both)
                .Select(FindByEntity).ToList();
        }
        public void DeclassifyAll(IClassifiable classifiable)
        {
            var related = relationSrv.GetRelated<TaxonEntity>(classifiable, RelationDirection.Both);
            foreach (var taxonEntity in related)
            {
                relationSrv.RemoveRelation(classifiable, taxonEntity);
            }
        }
        #endregion


        internal TaxonomyNode FindByEntity(TaxonEntity entity)
        {
            //TODO: optimize
            var path = "";
            TaxonEntity parent = entity;
            do
            {
                path = parent.Name + (String.IsNullOrEmpty(path) ? "" : "/" + path);
                parent = relationSrv.GetByRelated<TaxonEntity>(parent, RelationDirection.Forward, rootNode).FirstOrDefault();
            } while (parent != null && parent.Id!=rootNode.Id);

            var ret = FindOne(path);
            if(ret==null) throw new InvalidOperationException(
                String.Format("Entity not found in tree ({0}:{1}", entity.Id, path));
            return ret;
        }

        internal void UpdateEntity(TaxonomyNode taxon)
        {
            if (taxon.IsDetached) throw new Exception(String.Format("Taxon '{0}' is detached", taxon.Name));
            repo.Update(taxon.entity);
        }

        internal TaxonomyNode AddChildTo(TaxonomyNode taxon, string name, string description)
        {
            if (taxon.IsDetached) throw new Exception(String.Format("Taxon '{0}' is detached", taxon.Name));
            if (taxon.Children.Any(x => x.Name == name)) throw new DataLayerException("Duplicate taxon name in node");
            var entity = new TaxonEntity()
            {
                Name = name,
                Description = description
            };
            repo.Save(entity);
            relationSrv.AddRelation(taxon.entity, entity, RelationDirection.Forward, rootNode);
            var newNode = new TaxonomyNode(this, entity) { parent = taxon };
            taxon.children.Add(newNode);
            return newNode;
        }
        internal void RemoveNode(TaxonomyNode taxonomyNode, bool includeChildren)
        {
            if (taxonomyNode.IsDetached) throw new Exception(String.Format("Taxon '{0}' is detached", taxonomyNode.Name));
            if (taxonomyNode == rootNode) throw new Exception("Can't remove root");
            if (!includeChildren && taxonomyNode.Children.Count > 0) throw new DataLayerException("Taxon have children");

            var nodesToRemove = new List<TaxonomyNode>(taxonomyNode.GetAllChildren());
            nodesToRemove.Add(taxonomyNode);
            foreach (var node in nodesToRemove)
            {
                node.IsDetached = true;
                repo.Delete(node.entity);
            }
            taxonomyNode.Parent.children.Remove(taxonomyNode);
        }
        #endregion
    }
}
