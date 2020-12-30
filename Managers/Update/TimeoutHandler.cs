using System;

namespace Divine.SDK.Managers.Update
{
    public class TimeoutHandler : InvokeHandler
    {
        private uint timeout;

        public TimeoutHandler(uint timeout, bool fromNow = false)
        {
            Timeout = timeout;
            NextUpdate = fromNow ? DateTime.Now.AddMilliseconds(Timeout) : DateTime.Now;
        }

        public uint Timeout
        {
            get
            {
                return timeout;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Timeout");
                }

                timeout = value;
            }
        }

        protected bool HasTimeout
        {
            get
            {
                if (Timeout > 0)
                {
                    return DateTime.Now > NextUpdate;
                }

                return true;
            }
        }

        protected DateTime NextUpdate { get; set; }

        public override bool Invoke(Action callback)
        {
            if (HasTimeout)
            {
                NextUpdate = DateTime.Now.AddMilliseconds(Timeout);
                callback.Invoke();
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"Timer[{Timeout}]";
        }
    }
}