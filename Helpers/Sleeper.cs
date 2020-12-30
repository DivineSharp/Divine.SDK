namespace Divine.SDK.Helpers
{
    public sealed class Sleeper
    {
        private bool isSleeping;

        private float sleepTime;

        public bool Sleeping
        {
            get
            {
                if (!isSleeping)
                {
                    return false;
                }

                isSleeping = GameManager.RawGameTime * 1000 < sleepTime;
                return isSleeping;
            }
        }

        public void Reset()
        {
            sleepTime = 0;
            isSleeping = false;
        }

        public void Sleep(float milliseconds)
        {
            sleepTime = GameManager.RawGameTime * 1000 + milliseconds;
            isSleeping = true;
        }
    }
}