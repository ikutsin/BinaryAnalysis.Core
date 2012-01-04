using System.Collections;
using System.IO;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Box.Presentations
{
    public class NetDataContractBoxPresentation<E> : AbstractBoxStreamPresentation<E>
    {
        NetDataContractSerializer serializer = new NetDataContractSerializer();
        public override IBox<E> FromStream(Stream stream)
        {
            return serializer.Deserialize(stream) as IBox<E>;
        }

        protected override AbstractBoxPresentation CreateNonGenericInstance()
        {
            return new NetDataContractBoxPresentation();
        }
    }
}
