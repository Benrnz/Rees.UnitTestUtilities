using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <param name="output">
        ///     An optional override to use, to output the text while reading it.
        ///     If not specified, Console.WriteLine will be used.
        /// </param>
        /// <returns>A string array split by new lines from the text file.</returns>
        public static IEnumerable<string> ExtractEmbeddedResourceAsLines(
            this Assembly assembly,
            string resourceName,
            Action<string> output = null)
        {
            var text = assembly.ExtractEmbeddedResourceAsText(resourceName, output);
            output?.Invoke(text);
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
        /// <param name="output">
        ///     An optional override to use, to output the text while reading it.
        ///     If not specified, Console.WriteLine will be used.
        /// </param>
        /// <returns>A string containing the contents of the text file.</returns>
        public static string ExtractEmbeddedResourceAsText(
            this Assembly assembly, 
            string resourceName,
            Action<string> output = null)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new MissingManifestResourceException("Cannot find resource named: " + resourceName);
                }

                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                output?.Invoke(text);
                return text;
            }
        }

        /// <summary>
        ///     List all available Embedded Resources in an assembly that can be accessed. The resource name given can be used to
        ///     access the resource.
        /// </summary>
        /// <param name="assembly">The assembly in which to find the embedded resource.</param>
        /// <param name="output">
        ///     An optional override to use to output to. If not specified, Console.WriteLine will be used.
        /// </param>
        public static void ListAllEmbeddedResources(this Assembly assembly, Action<string> output = null)
        {
            // this line of code is useful to figure out the name Vs has given the resource! The name is case sensitive.
            assembly.GetManifestResourceNames().ToList().ForEach(output);
        }
    }
}