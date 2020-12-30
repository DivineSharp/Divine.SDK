using System;

namespace Divine.SDK.Managers.Update
{
    public class UpdateHandler
    {
        public UpdateHandler(Action callback, InvokeHandler executor, bool isEnabled = true)
            : this($"{callback?.Method.DeclaringType?.Name}.{callback?.Method.Name}", callback, executor, isEnabled)
        {
        }

        public UpdateHandler(string name, Action callback, InvokeHandler executor, bool isEnabled = true)
        {
            Name = name;
            Callback = callback;
            Executor = executor;
            IsEnabled = isEnabled;
        }

        public Action Callback { get; }

        public InvokeHandler Executor { get; set; }

        public bool IsEnabled { get; set; }

        public string Name { get; }

        public virtual bool Invoke()
        {
            if (!IsEnabled)
            {
                return false;
            }

            return Executor.Invoke(Callback);
        }

        public override string ToString()
        {
            return $"{Executor}[{Name}]";
        }
    }
}