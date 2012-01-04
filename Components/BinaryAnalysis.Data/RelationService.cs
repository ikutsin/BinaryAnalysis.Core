using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Mixin;

namespace BinaryAnalysis.Data
{
    public class RelationService
    {
        private readonly RelationRepository relationRepo;

        public RelationService(RelationRepository relationRepo)
        {
            this.relationRepo = relationRepo;
        }

        public void SetRelation(IClassifiable relatable, IClassifiable related, RelationDirection direction = RelationDirection.Undefined, TaxonomyNode type = null)
        {
            foreach (var relEntity in relationRepo.GetRelated(relatable, direction, type == null ? null : type.entity))
            {
                relationRepo.Delete(relEntity);
            }
            RelateInternal(relatable, related, direction, type);
        }

        public void AddRelation(IClassifiable relatable, IClassifiable related, RelationDirection direction = RelationDirection.Undefined, TaxonomyNode type = null)
        {
            RelateInternal(relatable, related, direction, type);
            RelateInternal(related, relatable, BackDirection(direction), type);
        }

        public int RemoveRelations(IClassifiable relatable, TaxonomyNode type = null)
        {
            var rels = relationRepo.GetAllRelationsFor(relatable).AsEnumerable();
            if (type != null) rels = rels.Where(r => r.Type!=null && r.Type.Id == type.entity.Id);
            relationRepo.DeleteAll(rels);
            return rels.Count();
        }

        public void RemoveRelation(IClassifiable relatable, IClassifiable related, TaxonomyNode type = null)
        {
            AddRelation(related, relatable, type:type);
        }
        protected void RelateInternal(IClassifiable relatable, IClassifiable related, RelationDirection direction, TaxonomyNode type)
        {
            var currentRelation = GetRelation(relatable, related, type);
            if (direction == RelationDirection.Undefined)
            {
                if (currentRelation != null)
                {
                    relationRepo.Delete(currentRelation);
                }
                //do nothing if direction is undefined
            }
            else
            {
                if (currentRelation == null)
                {
                    currentRelation = RelationEntity.Create(relatable, related, type==null?null:type.entity, direction);
                    relationRepo.Save(currentRelation);
                }
                else
                {
                    currentRelation.Direction = direction;
                    relationRepo.Update(currentRelation);
                }               
            }
        }

        public RelationDirection BackDirection(RelationDirection direction)
        {
            if (direction == RelationDirection.Undefined) return RelationDirection.Undefined;
            else if (direction == RelationDirection.Forward) return RelationDirection.Back;
            else if (direction == RelationDirection.Back) return RelationDirection.Forward;
            else if (direction == RelationDirection.Both) return RelationDirection.Both;
            throw new Exception("Unknown direction");
        }

        public RelationEntity GetRelation(IClassifiable relatable, IClassifiable related, TaxonomyNode type = null)
        {
            return relationRepo.GetRelation(relatable, related, type==null?null:type.entity);
        }
        public bool IsRelated(IClassifiable relatable, IClassifiable related, TaxonomyNode type = null)
        {
            return GetRelation(relatable, related, type)!=null;
        }

        public IList<T> GetRelated<T>(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonomyNode type = null) where T : Entity, IClassifiable
        {
            return relationRepo.GetRelated<T>(classifiable, direction, type==null?null:type.entity);
        }
        public IList<T> GetByRelated<T>(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonomyNode type = null) where T : Entity, IClassifiable
        {
            return relationRepo.GetByRelated<T>(classifiable, direction, type==null?null:type.entity);
        }

        public IList<RelatedIdsResult> GetRelatedIds(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonomyNode type = null)
        {
            return relationRepo.GetRelatedIds(classifiable, direction, type == null ? null : type.entity);
        }
        public IList<RelatedIdsResult> GetByRelatedIds(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonomyNode type = null)
        {
            return relationRepo.GetByRelatedIds(classifiable, direction, type == null ? null : type.entity);
        }
    }
}
