using System.IO;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Box.Presentations
{
    public class NetDataContractBoxPresentation : AbstractBoxStreamPresentation
    {
        NetDataContractSerializer serializer = new NetDataContractSerializer();
        public override void ToStream(IBox box, Stream stream)
        {
            serializer.Serialize(stream, box);
        }
    }
   
}
