using System.Collections;
using System.IO;

namespace BinaryAnalysis.Box.Presentations
{
    public interface IBoxPresentation<E> : IBoxPresentation
    {
        IBox<E> FromStream(Stream stream);
        IBox<E> FromString(string str);
        IBox<E> FromBytes(byte[] data);
        IBox<E> FromFile(string path);
    }
    public abstract class AbstractBoxPresentation<E> : IBoxPresentation<E>
    {
        //public abstract void ToStream(Stream stream);
        //public abstract string AsString();
        public abstract IBox<E> FromStream(Stream stream);
        public abstract IBox<E> FromString(string str);
        public abstract IBox<E> FromBytes(byte[] data);

        public AbstractBoxPresentation()
        {
            NonGenericInstance = CreateNonGenericInstance();
        }

        public IBox<E> FromFile(string path)
        {
            using (var file = File.OpenRead(path))
            {
                return FromStream(file);
            }
        }

        protected abstract AbstractBoxPresentation CreateNonGenericInstance();
        protected AbstractBoxPresentation NonGenericInstance { get; private set; }

        public string AsString(IBox box)
        {
            return NonGenericInstance.AsString(box);
        }

        public byte[] AsBytes(IBox box)
        {
            return NonGenericInstance.AsBytes(box);
        }

        public void ToStream(IBox box, Stream stream)
        {
            NonGenericInstance.ToStream(box, stream);
        }


        public void ToFile(IBox box, string path)
        {
            NonGenericInstance.ToFile(box, path);
        }
    }
}
