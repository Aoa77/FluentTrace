using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FluentTrace.NetStandard
{
    public sealed class Call
    {
        private readonly List<Data> _data;
        private readonly Stack<Frame> _stack;

        public IEnumerable<Data> Data => _data;
        public IEnumerable<Frame> Stack => _stack;

        internal Call(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            _data = new List<Data>();
            _stack = new Stack<Frame>();
            _ = WithCapture(file, func, line);
        }

        public Call WithCapture(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            _stack.Push(new Frame(file, func, line));
            return this;
        }

        public Call WithData(params (string Name, object Value, Type Type)[] data)
        {
            _data.AddRange(data.Select(x => new Data(x.Name, x.Value, x.Type)));
            return this;
        }

        public Call WithModel<T>(T model)
        {
            return WithModel(model, typeof(T));
        }

        public Call WithModel<T>(string name, T model)
        {
            return WithModel(name, model, typeof(T));
        }

        public Call WithModel(object model, Type type)
        {
            return WithModel(nameof(model), model, type);
        }

        public Call WithModel(string name, object model, Type type)
        {
            _data.Add(new Data(name, model, type));

            model.GetType().GetProperties().ToList().ForEach(x =>
            {
                _data.Add(new Data(
                    x.Name, x.GetValue(model),
                    x.PropertyType,
                    prefix: "+ "));
            });

            return this;
        }

        public void Write()
        {
            Trace.Write(this);
        }

        public void Write(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            _data.Insert(0, new Data(nameof(message), message, message.GetType()));
            Trace.Write(this);
        }
    }
}
