using System;
using System.Linq;
using System.Reflection;

namespace Divine.SDK.Managers.Log
{
    public static class MetadataExtensions
    {
        public static AssemblyMetadata GetMetadata(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var attributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                return null;
            }

            var metadata = new AssemblyMetadata();

            foreach (var data in attributes)
            {
                try
                {
                    switch (data.Key)
                    {
                        case "Id":
                            {
                                metadata.Id = data.Value;
                            }
                            break;

                        case "Commit":
                            {
                                metadata.Commit = data.Value;
                            }
                            break;

                        case "Channel":
                            {
                                metadata.Channel = data.Value;
                            }
                            break;

                        case "Version":
                            {
                                metadata.Version = data.Value;
                            }
                            break;

                        case "Build":
                            {
                                metadata.Build = data.Value;
                            }
                            break;

                        case "SentryProject":
                            {
                                metadata.SentryProject = data.Value;
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Assembly {assembly.FullName} contains an invalid {nameof(AssemblyMetadataAttribute)} {data.Key}={data.Value}", e);
                }
            }

            return metadata;
        }
    }
}
