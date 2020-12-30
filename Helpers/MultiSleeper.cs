using System.Collections.Generic;

namespace Divine.SDK.Helpers
{
    public static class MultiSleeper<T>
    {
        private static readonly Dictionary<T, Sleeper> Sleepers = new Dictionary<T, Sleeper>();

        public static Sleeper Sleeper(T key)
        {
            if (!Sleepers.TryGetValue(key, out var sleeper))
            {
                sleeper = new Sleeper();
                Sleepers[key] = sleeper;
            }

            return sleeper;
        }

        public static void Reset(T key)
        {
            Sleeper(key).Reset();
        }

        public static void Sleep(T key, float milliseconds)
        {
            Sleeper(key).Sleep(milliseconds);
        }

        public static bool Sleeping(T key)
        {
            return Sleeper(key).Sleeping;
        }
    }
}