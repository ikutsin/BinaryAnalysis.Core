using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BinaryAnalysis.Box
{
    public interface IBox<T> : IList<T>, IBox
    {

    }
    public interface IBox : IList
    {

    }
}
