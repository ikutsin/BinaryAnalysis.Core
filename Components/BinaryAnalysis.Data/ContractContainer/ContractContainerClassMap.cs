using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Data.ContractContainer
{
    public class ContractContainerClassMap<T> : EntityClassMap<T> where T : ContractContainerEntity
    {
        public ContractContainerClassMap()
        {
            Map(x => x.Contract).Nullable();//.CustomType("BinaryBlob");
            Map(x => x.ContractType).Nullable();//.CustomType("BinaryBlob");
            Map(x => x.StringValue).Nullable();//.CustomType("BinaryBlob");
            Map(x => x.NumericValue).Nullable();
        }
    }
}
