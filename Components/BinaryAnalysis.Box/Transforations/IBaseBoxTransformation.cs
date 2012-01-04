using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Box.Transforations
{
    public interface IBaseBoxTransformation<T>
    {
        IBox<T> ToBox();
        void Transform(IBox<T> box);
    }
}
