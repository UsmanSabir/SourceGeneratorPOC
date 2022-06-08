using System.IO;
using System.Reflection;

namespace GeneratorLib
{
    static class ResourceHelper
    {
        public static string GetResourceFileContentAsString(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "GeneratorLib." + fileName; //

            string resource = null;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    resource = reader.ReadToEnd();
                }
            }
            return resource;
        }
    }
}
