using System.Collections;
using System.IO;

namespace BinaryAnalysis.Box.Presentations
{
    public interface IBoxPresentation
    {
        string AsString(IBox box);
        byte[] AsBytes(IBox box);
        void ToStream(IBox box, System.IO.Stream stream);
        void ToFile(IBox box, string path);
    }


    public abstract class AbstractBoxPresentation : IBoxPresentation
    {
        public abstract string AsString(IBox box);
        public abstract byte[] AsBytes(IBox box);
        public abstract void ToStream(IBox box, Stream stream);
        //protected 
        public void ToFile(IBox box, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Delete(path);
            using (var file = File.OpenWrite(path))
            {
                ToStream(box, file);
            }
            //File.WriteAllText(path, AsString());
        }
    }
}
