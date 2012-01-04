using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;

namespace BinaryAnalysis.Data.Settings
{
    public interface ISettingsHolder : IClassifiable
    {
        SettingsEntity Settings { get; set; }
    }
}
