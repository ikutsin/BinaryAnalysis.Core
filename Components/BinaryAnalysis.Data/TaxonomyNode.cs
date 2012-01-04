using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Settings;

namespace BinaryAnalysis.Data
{
    public class TaxonomyNode : IClassifiable
    {
        TaxonomyTree tree;
        internal TaxonEntity entity;

        public string ObjectName { get { return TaxonEntity.OBJECT_NAME; } }
        public bool IsDetached { get; internal set; }

        internal TaxonomyNode(TaxonomyTree tree, TaxonEntity entity)
        {
            this.tree = tree;
            this.entity = entity;
            this.IsDetached = false;
            //tree.log.Debug("Node init:" + Name);
        }

        public int Id { get { return entity.Id; } }
        public string Name
        {
            get { return entity.Name; }
            set
            {
                entity.Name = value;
                tree.UpdateEntity(this);
            }
        }

        public string Description
        {
            get { return entity.Description; }
            set
            {
                entity.Description = value;
                tree.UpdateEntity(this);
            }
        }

        internal TaxonomyNode parent;
        public TaxonomyNode Parent
        {
            get
            {
                if (parent == null && this!=tree.Root)
                {
                    parent = tree.LoadParent(this);
                }
                return parent;
            }
        }
        internal List<TaxonomyNode> children;
        public IList<TaxonomyNode> Children
        {
            get
            {
                if (children == null)
                {
                    children = tree.LoadChildren(this);
                }
                return children.AsReadOnly();
            }
        }
        string path;
        public string Path
        {
            get
            {
                if (path == null)
                {
                    var parents = GetParents().Reverse();
                    if (parents.Count() > 0)
                    {
                        parents = parents.Skip(1).Concat(new List<TaxonomyNode> {this});
                        path = string.Join("/", parents.Select(x => x.Name));
                    }
                    else
                    {
                        //root
                        return "/";
                    }
                }
                return "/"+path;
            }
        }


        public TaxonomyNode this[int index] { get { return this.Children[index]; } }
        /// <summary>
        /// Nullable return
        /// </summary>
        /// <param name="name">name of the child</param>
        /// <returns>node</returns>
        public TaxonomyNode this[string name] { get { return this.Children.FirstOrDefault(x=>x.Name==name); } }

        public TaxonomyNode GetChild(string name)
        {
            return this.Children.First(x=>x.Name==name);
        }

        public TaxonomyNode AddChild(string name, string description = null)
        {
            return tree.AddChildTo(this, name, description);
        }

        public void Remove(bool includeChildren=false)
        {
            tree.RemoveNode(this, includeChildren);
        }

        //public TaxonomyNodeBoxMap WrapToContract()
        //{
        //    var node = this;
        //    return new TaxonomyNodeBoxMap()
        //    {
        //        Description = node.Description,
        //        Id = node.Id,
        //        Name = node.Name,
        //        Path = node.Path,
        //        ParentPath = node.Parent == null ? "" : node.Parent.Path
        //    };
        //}

        #region Helpers
        public IList<TaxonomyNode> GetAllChildren()
        {
            var ret = new List<TaxonomyNode>();
            ret.AddRange(Children);
            foreach (var child in Children) ret.AddRange(child.GetAllChildren());
            return ret.AsReadOnly();
        }
        public bool HasChild(string name)
        {
            return Children.Any(x => x.Name == name);
        }
        public TaxonomyNode GetOrAddChild(string name, string defaultDescription)
        {
            var ret = Children.FirstOrDefault(x => x.Name == name);
            if (ret != null) return ret;
            return AddChild(name, defaultDescription);
        }
        public IList<TaxonomyNode> GetParents()
        {
            var ret = new List<TaxonomyNode>();
            if (Parent != null)
            {
                ret.Add(Parent);
                ret.AddRange(Parent.GetParents());
            }
            return ret;
        }
        public override string ToString()
        {
            return path;
        }
        
        #endregion
    }
}
