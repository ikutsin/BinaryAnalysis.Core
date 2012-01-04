using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using BinaryAnalysis.Box;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Data.ContractContainer
{
    public abstract class ContractContainerEntity : Entity, IContractContainer
    {
        public virtual string StringValue { get; set; }
        public virtual decimal? NumericValue { get; set; }
        public virtual byte[] Contract { get; set; }
        public virtual string ContractType { get; set; }

        public virtual void SetValue(object val)
        {
            objectCache = null;
            StringValue = null;
            NumericValue = null;
            Contract = null;

            if (val is string)
            {
                StringValue = val.ToString();
                return;
            }

            if (val is ValueType)
            {
                var valType = val as ValueType;
                if (IsNumericType(valType) || val is Enum)
                {
                    NumericValue = Convert.ToDecimal(val);
                    return;
                }
                if (val is bool)
                {
                    NumericValue = ((bool)val) == true ? 1 : 0;
                    return;
                }
                //structs
                if (val is DateTime)
                {
                    NumericValue = ((DateTime)val).Ticks;
                    return;
                }
                //fall to contract
            }
            //contract
            if (val != null)
            {
                var box = BoxExtensions.CreateFor(val.GetType());
                box.Add(val);

                IBoxPresentation serializer = new BinaryBoxPresentation();
                Contract = serializer.AsBytes(box);
                ContractType = val.GetType().AssemblyQualifiedName;
            }
            else
            {
                ContractType = null;
                Contract = null;
            }
        }

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
                var presenterType = typeof(BinaryBoxPresentation<>).MakeGenericType(contractType);
                var presenter = Activator.CreateInstance(presenterType);
                var objectBox = presenterType.GetMethod("FromBytes").Invoke(presenter, new[] { Contract });
                if (contractType.IsAssignableFrom(objectBox.GetType())) objectCache = objectBox;
                else
                {
                    objectCache = ((IList)objectBox)[0];
                }
            }
            return (T) objectCache;
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
    }
}
