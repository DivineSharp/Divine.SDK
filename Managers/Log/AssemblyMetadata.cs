namespace Divine.SDK.Managers.Log
{
    public sealed class AssemblyMetadata
    {
        public string Build { get; internal set; }

        public string Channel { get; internal set; }

        public string Commit { get; internal set; }

        public string Id { get; internal set; }

        public string SentryProject { get; internal set; }

        public string Version { get; internal set; }
    }
}
