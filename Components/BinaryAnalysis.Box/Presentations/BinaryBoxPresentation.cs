using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Box.Presentations
{
    public class BinaryBoxPresentation<E> : AbstractBoxStreamPresentation<E>
    {
        protected IFormatter formatter = new BinaryFormatter();
        public override IBox<E> FromStream(Stream stream)
        {
            return formatter.Deserialize(stream) as IBox<E>;
        }

        protected override AbstractBoxPresentation CreateNonGenericInstance()
        {
            return new BinaryBoxPresentation();
        }
    }
}
