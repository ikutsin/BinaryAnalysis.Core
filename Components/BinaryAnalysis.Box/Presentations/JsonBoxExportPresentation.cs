using System.Text;
using Newtonsoft.Json;

namespace BinaryAnalysis.Box.Presentations
{

    public class JsonBoxPresentation : AbstractBoxPresentation
    {
        public override string AsString(IBox box)
        {
            return JsonConvert.SerializeObject(box, Formatting.Indented,
                new JsonSerializerSettings
                    {
                        //TypeNameHandling = TypeNameHandling.All,
                        //TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                    });
        }

        public override byte[] AsBytes(IBox box)
        {
            return Encoding.Default.GetBytes(AsString(box));
        }

        public override void ToStream(IBox box, System.IO.Stream stream)
        {
            var bytes = AsBytes(box);
            stream.Write(bytes, 0, bytes.Length);
        }

    }
   
   
}
