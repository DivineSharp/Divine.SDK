using System;
using System.Threading.Tasks;

namespace Divine.SDK.Helpers
{
    public static class Timer
    {
        static Timer()
        {
            var ticks = DateTime.UtcNow.Ticks;

            Time = (float)((DateTime.UtcNow.Ticks - ticks) * 1E-07);

            Task.Run(async () =>
            {
                while (true)
                {
                    Time = (float)((DateTime.UtcNow.Ticks - ticks) * 1E-07);
                    await Task.Delay(10);
                }
            });
        }

        public static float Time { get; private set; }
    }
}