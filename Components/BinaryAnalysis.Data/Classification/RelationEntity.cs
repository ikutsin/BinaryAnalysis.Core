using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using NHibernate.Validator.Constraints;

namespace BinaryAnalysis.Data.Classification
{
    [BoxTo(typeof(RelationBoxMap))]
    public class RelationEntity : Entity
    {
        public static void BoxDescription(RelationEntity e, RelationBoxMap eMap)
        {
            eMap.Description = e.ToString();
        }

        public static RelationEntity Create(IClassifiable relatable, IClassifiable related, TaxonEntity type = null, RelationDirection direction = RelationDirection.Undefined)
        {
            return new RelationEntity()
            {
                Type = type,
                Direction = direction,
                ObjectName = relatable.ObjectName,
                ObjectID = relatable.Id,
                RelatedObjectName = related.ObjectName,
                RelatedObjectID = related.Id,
            };
        }

        public virtual TaxonEntity Type { get; set; }
        public virtual RelationDirection Direction { get; set; }

        [NotNullNotEmpty, Length(30)]
        public virtual string ObjectName { get; set; }
        [Min(1)]
        public virtual int ObjectID { get; set; }

        [NotNullNotEmpty, Length(30)]
        public virtual string RelatedObjectName { get; set; }
        [Min(1)]
        public virtual int RelatedObjectID { get; set; }

        public override string ToString()
        {
            return String.Format("{0}({1}){4}{2}({3}) {5}",
                                 ObjectName, ObjectID, RelatedObjectName, RelatedObjectID,
                                 DirectionToString(Direction),
                                 Type == null ? "" : "(" + Type.Name + ")"
                );
        }
        string DirectionToString(RelationDirection dir)
        {
            switch (dir)
            {
                case RelationDirection.Back:
                    return "<--";
                case RelationDirection.Forward:
                    return "-->";
                case RelationDirection.Both:
                    return "<->";
                default:
                    return ">-<";
            }
        }
    }
}
