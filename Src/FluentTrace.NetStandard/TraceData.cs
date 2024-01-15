using System;

namespace FluentTrace.NetStandard
{
    public sealed class TraceData
    {
        internal TraceData(string name, object value, Type type, string prefix = null)
        {
            Prefix = prefix;
            ParamName = name;
            ParamType = type;
            Value = value ?? TraceLog.Configuration.NullDataDisplay;

            var nullable = Nullable.GetUnderlyingType(ParamType);
            if (nullable == null)
            {
                TypeName = ParamType.Name;
                return;
            }
            TypeName = nullable.Name + "?";
        }

        public string Prefix { get; }
        public string ParamName { get; }
        public Type ParamType { get; }
        public string TypeName { get; }
        public object Value { get; }
    }
}
