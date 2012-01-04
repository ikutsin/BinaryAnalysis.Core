using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Data.Box;
using Newtonsoft.Json;

namespace BinaryAnalysis.Data.ContractContainer
{
    [Serializable]
    [XmlType("ContractContainer")]
    [DataContract]
    [JsonObject]
    public class ContractContainerBoxMap : EntityBoxMap, IContractContainer
    {
        [DataMember, XmlElement, JsonProperty]
        public string StringValue { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public decimal? NumericValue { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public byte[] Contract { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual string ContractType { get; set; }

        public override string ToString()
        {
            return IsContract?GetContractString():GetValue().ToString();
        }

        public string GetContractString()
        {
            if (Contract == null) return null;
            using (MemoryStream stream = new MemoryStream(Contract))
            {
                stream.Position = 0;
                return Encoding.ASCII.GetString(stream.ToArray());
            }
        }

        #region Dup from ContractContainerEntity class
        private object objectCache = null;
        public virtual T GetValue<T>()
        {
            if (typeof(T) == typeof(String)) return (T)(object)StringValue;
            if (typeof(T).IsValueType)
            {
                if (typeof(T) == typeof(DateTime))
                {
                    return (T)(object)new DateTime((long)NumericValue.Value);
                }
                var numVal = NumericValue.Value;
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)Convert.ToInt32(numVal);
                }
                return (T)(object)numVal;
            }

            //contract
            if (objectCache == null)
            {
                var contractType = Type.GetType(ContractType);
                var presenterType = typeof (BinaryBoxPresentation<>).MakeGenericType(contractType);
                var presenter = Activator.CreateInstance(presenterType);
                var objectBox = presenterType.GetMethod("FromBytes").Invoke(presenter, new[] {Contract});
                if (contractType.IsAssignableFrom(objectBox.GetType())) objectCache = objectBox;
                else
                {
                    objectCache = ((IList) objectBox)[0];
                }
            }
            return (T)objectCache;
        }

        public virtual object GetValue()
        {
            if (IsNumeric)
            {
                return NumericValue.Value;
            }
            if (IsString)
            {
                return StringValue;
            }
            return GetValue<object>();
        }

        public virtual bool IsContract
        {
            get { return (Contract != null); }
        }
        public virtual bool IsNumeric
        {
            get { return NumericValue.HasValue; }
        }
        public virtual bool IsString
        {
            get { return !string.IsNullOrEmpty(StringValue); }
        }

        protected static bool IsNumericType(ValueType value)
        {
            if (!(value is Byte ||
                    value is Int16 ||
                    value is Int32 ||
                    value is Int64 ||
                    value is SByte ||
                    value is UInt16 ||
                    value is UInt32 ||
                    value is UInt64 ||
                //value is BigInteger ||
                    value is Decimal ||
                    value is Double ||
                    value is Single))
                return false;
            else
                return true;
        }
        
        #endregion
    }
}
