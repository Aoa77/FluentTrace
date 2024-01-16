using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace FluentTrace.NetStandard
{
    public sealed class Capture
    {
        private readonly List<TraceData> _data;
        private readonly Stack<CallSite> _stack;

        public IEnumerable<TraceData> Data => _data;
        public IEnumerable<CallSite> Stack => _stack;

        public bool Flushed { get; private set; }

        internal Capture(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            _data = new List<TraceData>();
            _stack = new Stack<CallSite>();
            _ = WithCallSite(file, func, line);
        }

        /// <summary>
        /// Writes the captured call stack and data to a log file.
        /// </summary>
        public void Flush()
        {
            TraceLog.Flush(this);
        }

        /// <summary>
        /// Pushes a call site onto the call stack of this capture.
        /// </summary>
        public Capture WithCallSite(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            _stack.Push(new CallSite(file, func, line));
            return this;
        }

        /// <summary>
        /// Adds data to this capture.
        /// </summary>
        public Capture WithData(params (string Name, object Value, Type Type)[] data)
        {
            _data.AddRange(data.Select(x => new TraceData(x.Name, x.Value, x.Type)));
            return this;
        }

        /// <summary>
        /// Adds data to this capture.
        /// </summary>
        public Capture WithJson<T>(T model)
        {
            return WithData((nameof(model),
                JsonSerializer.Serialize(model, TraceLog.Config.Json),
                typeof(T)));
        }

        /// <summary>
        /// Adds data to this capture.
        /// </summary>
        public Capture WithMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            _data.Add(new TraceData(
                name: nameof(message),
                value: message,
                type: message.GetType()));

            return this;
        }

        /// <summary>
        /// Adds data to this capture.
        /// </summary>
        public Capture WithModel<T>(T model)
        {
            return WithModel(model, typeof(T));
        }

        /// <summary>
        /// Adds data to this capture.
        /// </summary>
        public Capture WithModel<T>(string name, T model)
        {
            return WithModel(name, model, typeof(T));
        }

        /// <summary>
        /// Adds data to this capture.
        /// </summary>
        public Capture WithModel(object model, Type type)
        {
            return WithModel(nameof(model), model, type);
        }

        /// <summary>
        /// Adds data to this capture.
        /// </summary>
        public Capture WithModel(string name, object model, Type type)
        {
            _data.Add(new TraceData(name, model, type));

            model.GetType().GetProperties().ToList().ForEach(x =>
            {
                _data.Add(new TraceData(
                    name: x.Name, value: x.GetValue(model),
                    type: x.PropertyType,
                    prefix: "+ "));
            });

            return this;
        }
    }
}
