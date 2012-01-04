using System.Collections;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace BinaryAnalysis.Box.Presentations
{
    public class JsonBoxPresentation<E> : AbstractBoxPresentation<E>
    {
        public override IBox<E> FromStream(Stream stream)
        {
            stream.Position = 0;
            var buff = new byte[stream.Length];
            stream.Read(buff, 0, (int)stream.Length);
            return FromBytes(buff);
        }

        public override IBox<E> FromString(string str)
        {
            var ser = new JsonSerializer();
            return JsonConvert.DeserializeObject(str, typeof (IBox<E>)) as IBox<E>;
        }

        public override IBox<E> FromBytes(byte[] data)
        {
            return FromString(Encoding.Default.GetString(data));
        }

        protected override AbstractBoxPresentation CreateNonGenericInstance()
        {
            return new JsonBoxPresentation();
        }
    }

}
