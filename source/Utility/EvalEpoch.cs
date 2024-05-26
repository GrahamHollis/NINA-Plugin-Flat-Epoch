using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Settings = Hologram.NINA.FlatEpoch.Properties.Settings;


namespace Hologram.NINA.FlatEpoch.Utility
{
    class EvalEpochFiles {

        public static List<Dictionary<string, string>> ParseImageDirectory(string imagesDirectory, string key, string epoch, string frametype) {
            var uniqueCombinations = new List<Dictionary<string, string>>();
            char[] delimiters = new char[] { '_', '-' };

            // Check possible delimiters
            foreach (char delimiter in delimiters) {
                string targetDirectoryKeyValue = $"{key}{delimiter}{epoch}";
                string baseDirectory = Path.Combine(imagesDirectory, frametype);

                // Check if the target dir exists
                if (Directory.Exists(baseDirectory)) {
                    // Recursively walk through the directory structure
                    WalkDirectory(baseDirectory, targetDirectoryKeyValue, uniqueCombinations);
                }
            }
            return uniqueCombinations;
        }

        static void WalkDirectory(string directory, string targetDirectoryKeyValue, List<Dictionary<string, string>> uniqueCombinations) {
            foreach (string subdirectory in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories)) {
                if (Path.GetFileName(subdirectory).Contains(targetDirectoryKeyValue)) {
                    ProcessFilesInNestedDirectories(subdirectory, uniqueCombinations);
                }
            }
        }

        static void ProcessFilesInNestedDirectories(string basefiledirectory, List<Dictionary<string, string>> uniqueCombinations) {
            // Process files in directory
            foreach (string file in Directory.GetFiles(basefiledirectory, "*", SearchOption.AllDirectories)) {
                string[] filenameParts = Path.GetFileNameWithoutExtension(file).Split(new char[] { '_', '-', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                var filterDict = new Dictionary<string, string>();
                for (int i = 0; i < filenameParts.Length; i++) {
                    if (new HashSet<string> { Settings.Default.labFilter,
                                              Settings.Default.labGain,
                                              Settings.Default.labOffset,
                                              Settings.Default.labBin,
                                              Settings.Default.labReadOut,
                                              Settings.Default.labRot}.Contains(filenameParts[i]) && i < filenameParts.Length - 1) {
                        filterDict[filenameParts[i]] = filenameParts[i + 1];
                    }
                }
                if (!uniqueCombinations.Any(d => DictionaryEquals(d, filterDict))) {
                    uniqueCombinations.Add(filterDict);
                }
            }
        }

        // Check if dictionaries are equal
        static bool DictionaryEquals(Dictionary<string, string> x, Dictionary<string, string> y) {
            if (x == null || y == null)
                return x == y;

            return x.Count == y.Count && x.All(pair => y.TryGetValue(pair.Key, out var value) && pair.Value == value);
        }

    }


    class DictionaryComparer : IEqualityComparer<Dictionary<string, string>> {
        public bool Equals(Dictionary<string, string> x, Dictionary<string, string> y) {
            if (x == null || y == null)
                return x == y;

            return x.Count == y.Count && x.All(pair => y.TryGetValue(pair.Key, out var value) && pair.Value == value);
        }

        public int GetHashCode(Dictionary<string, string> obj) {
            unchecked {
                int hash = 17;
                foreach (var kvp in obj) {
                    hash = hash * 23 + kvp.Key.GetHashCode();
                    hash = hash * 23 + kvp.Value?.GetHashCode() ?? 0;
                }
                return hash;
            }
        }
    }

}
