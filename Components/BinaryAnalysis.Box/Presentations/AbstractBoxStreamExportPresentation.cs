using System.Text;
using System.IO;

namespace BinaryAnalysis.Box.Presentations
{
    public abstract class AbstractBoxStreamPresentation : AbstractBoxPresentation
    {
        public override string AsString(IBox box)
        {
            return Encoding.Default.GetString(AsBytes(box));
        }

        public override byte[] AsBytes(IBox box)
        {
            using (var memoryStream = new MemoryStream())
            {
                ToStream(box, memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
    
}
