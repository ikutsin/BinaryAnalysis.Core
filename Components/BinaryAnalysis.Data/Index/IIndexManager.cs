using System;
namespace BinaryAnalysis.Data.Index
{
    public interface IIndexManager
    {
        void Fill();
        void Optimize();
        void Purge();
    }
}
