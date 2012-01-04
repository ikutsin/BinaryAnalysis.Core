using System.IO;
using System.Collections;

namespace BinaryAnalysis.Box.Presentations
{
    public class DumperBoxPresentation : AbstractBoxStreamPresentation
    {
        public override void ToStream(IBox box, Stream stream)
        {
            using (TextWriter writer = new StreamWriter(stream))
            {
                Dumper.Dump(box, writer);
                foreach (var o in box)
                {
                    Dumper.Dump(o, writer);
                }
            }
        }
    }
   
   
}
