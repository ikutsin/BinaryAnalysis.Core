using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Box.Presentations
{
    public class BinaryBoxPresentation : AbstractBoxStreamPresentation
    {
        protected IFormatter formatter = new BinaryFormatter();
        public override void ToStream(IBox box, Stream stream)
        {
            formatter.Serialize(stream, box);
        }
    }
  
}
