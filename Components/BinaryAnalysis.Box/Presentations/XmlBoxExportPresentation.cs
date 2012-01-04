using System.Xml.Serialization;
using System.IO;

namespace BinaryAnalysis.Box.Presentations
{
    public class XmlBoxPresentation : AbstractBoxStreamPresentation
    {
        public override void ToStream(IBox box, Stream stream)
        {
            var serializer = new XmlSerializer(box.GetType());
            serializer.Serialize(stream, box);
        }
    }
    
}
