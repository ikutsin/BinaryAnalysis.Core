using System.Text;
using System.IO;

namespace BinaryAnalysis.Box.Presentations
{
    public abstract class AbstractBoxStreamPresentation<E> : AbstractBoxPresentation<E>
    {
        public override IBox<E> FromString(string str)
        {
            return FromBytes(Encoding.Default.GetBytes(str));
        }
        public override IBox<E> FromBytes(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                return FromStream(memoryStream);
            }
        }

    }
}
