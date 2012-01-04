using System.Collections;
using System.Xml.Serialization;
using System.IO;

namespace BinaryAnalysis.Box.Presentations
{
    public class XmlBoxPresentation<E> : AbstractBoxStreamPresentation<E>
    {
        public override IBox<E> FromStream(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Box<E>));
            return serializer.Deserialize(stream) as IBox<E>;
        }

        protected override AbstractBoxPresentation CreateNonGenericInstance()
        {
            return new XmlBoxPresentation();
        }
    }
}
