using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FluentTrace.NetStandard
{
    public sealed class CallStack
    {
        private readonly List<TraceData> _data;
        private readonly Stack<CallSiteRecord> _stack;

        public IEnumerable<TraceData> Data => _data;
        public IEnumerable<CallSiteRecord> Stack => _stack;

        internal CallStack(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            _data = new List<TraceData>();
            _stack = new Stack<CallSiteRecord>();
            _ = WithCapture(file, func, line);
        }

        public CallStack WithCapture(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            _stack.Push(new CallSiteRecord(file, func, line));
            return this;
        }

        public CallStack WithData(params (string Name, object Value, Type Type)[] data)
        {
            _data.AddRange(data.Select(x => new TraceData(x.Name, x.Value, x.Type)));
            return this;
        }

        public CallStack WithModel<T>(T model)
        {
            return WithModel(model, typeof(T));
        }

        public CallStack WithModel<T>(string name, T model)
        {
            return WithModel(name, model, typeof(T));
        }

        public CallStack WithModel(object model, Type type)
        {
            return WithModel(nameof(model), model, type);
        }

        public CallStack WithModel(string name, object model, Type type)
        {
            _data.Add(new TraceData(name, model, type));

            model.GetType().GetProperties().ToList().ForEach(x =>
            {
                _data.Add(new TraceData(
                    x.Name, x.GetValue(model),
                    x.PropertyType,
                    prefix: "+ "));
            });

            return this;
        }

        public void Write()
        {
            CompiledTrace.Write(this);
        }

        public void Write(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            _data.Insert(0, new TraceData(nameof(message), message, message.GetType()));
            CompiledTrace.Write(this);
        }
    }
}
