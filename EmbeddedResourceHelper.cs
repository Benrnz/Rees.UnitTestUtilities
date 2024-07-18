using System.Reflection;
using System.Resources;

namespace Rees.UnitTestUtilities
{
    /// <summary>
    ///     A helper class that provides extensions to the <see cref="Assembly" /> class to extract embedded resources to use
    ///     during unit testing.
    /// </summary>
    public static class EmbeddedResourceHelper
    {
        /// <summary>
        ///     Accesses a embedded resource text file included in the assembly and returns it as a set of lines.
        /// </summary>
        /// <param name="assembly">The assembly in which to find the embedded resource.</param>
        /// <param name="resourceName">
        ///     The resource name of the file. This matches its filename and path from the root of the solution.
        ///     For example "AssemblyName.FolderName(s).EmbeddedFile.csv";
        /// </param>
        /// <param name="outputText">Optionally output the text file to the output console before returning.</param>
        /// <param name="outputOverride">
        ///     An optional override to use to output to when using the <paramref name="outputText" />
        ///     option. If not specified, Console.WriteLine will be used.
        /// </param>
        /// <returns>A string array split by new lines from the text file.</returns>
        public static IEnumerable<string> ExtractEmbeddedResourceAsLines(
            this Assembly assembly,
            string resourceName,
            bool outputText = false,
            Action<string>? outputOverride = null)
        {
            var text = assembly.ExtractEmbeddedResourceAsText(resourceName, outputText);
            if (outputText)
            {
                if (outputOverride == null)
                {
                    Console.WriteLine(text);
                }
                else
                {
                    outputOverride(text);
                }
            }
            return text.SplitLines();
        }

        /// <summary>
        ///     Accesses a embedded resource text file included in the assembly and returns it as a set of lines.
        /// </summary>
        /// <param name="assembly">The assembly in which to find the embedded resource.</param>
        /// <param name="resourceName">
        ///     The resource name of the file. This matches its filename and path from the root of the solution.
        ///     For example "AssemblyName.FolderName(s).EmbeddedFile.csv";
        /// </param>
        /// <param name="outputText">Optionally output the text file to the output console before returning.</param>
        /// <param name="outputOverride">
        ///     An optional override to use to output to when using the <paramref name="outputText" />
        ///     option. If not specified, Console.WriteLine will be used.
        /// </param>
        /// <returns>A string containing the contents of the text file.</returns>
        public static string ExtractEmbeddedResourceAsText(
            this Assembly assembly, 
            string resourceName,
            bool outputText = false,
            Action<string>? outputOverride = null)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new MissingManifestResourceException("Cannot find resource named: " + resourceName);
                }

                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                if (outputOverride == null)
                {
                    Console.WriteLine(text);
                }
                else
                {
                    outputOverride(text);
                }
                return text;
            }
        }

        /// <summary>
        ///     List all available Embedded Resources in an assembly that can be accessed. The resource name given can be used to
        ///     access the resource.
        /// </summary>
        /// <param name="assembly">The assembly in which to find the embedded resource.</param>
        /// <param name="outputOverride">
        ///     An optional override to use to output to. If not specified, Console.WriteLine will be used.
        /// </param>
        public static void ListAllEmbeddedResources(this Assembly assembly, Action<string>? outputOverride = null)
        {
            // this line of code is useful to figure out the name Vs has given the resource! The name is case sensitive.
            assembly.GetManifestResourceNames().ToList().ForEach(t =>
            {
                if (outputOverride == null)
                {
                    Console.WriteLine(t);
                }
                else
                {
                    outputOverride(t);
                }
            });
        }
    }
}