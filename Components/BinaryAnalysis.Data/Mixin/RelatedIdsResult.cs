namespace BinaryAnalysis.Data.Mixin
{
    public class RelatedIdsResult : IRelationMixin
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}