using NINA.Core.Model.Equipment;
using NINA.Profile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hologram.NINA.FlatEpoch.Utility {

    public class Validate {

        public static (bool,string) CheckFilePattern(string filepattern, string epochkey, string filterkey, string gainkey, string offsetkey, string binkey, string readoutkey, string rotkey, string nrflats, string currepoch) {
            // Define the rules as regex
            //string rule1_1 = @"(?:^|[_\-\\])\$\$IMAGETYPE\$\$[_\-\\].*?" + Regex.Escape(epochkey);
            string rule1_1 = @"^\$\$IMAGETYPE\$\$";
            string rule2_1 = @"^(?=.*[_\-\/\\]" + Regex.Escape(epochkey) + @"[_\-])(?!.*[_\-\/\\]" + Regex.Escape(epochkey) + @"[_\-].*[_\-\/\\]" + Regex.Escape(epochkey) + @"[_\-])";
            string rule2_2 = @"^(?=.*[_\-\/\\]" + Regex.Escape(filterkey) + @"[_\-])(?!.*[_\-\/\\]" + Regex.Escape(filterkey) + @"[_\-].*[_\-\/\\]" + Regex.Escape(filterkey) + @"[_\-])";
            string rule2_3 = @"^(?=.*[_\-\/\\]" + Regex.Escape(gainkey) + @"[_\-])(?!.*[_\-\/\\]" + Regex.Escape(gainkey) + @"[_\-].*[_\-\/\\]" + Regex.Escape(gainkey) + @"[_\-])";
            string rule2_4 = @"^(?=.*[_\-\/\\]" + Regex.Escape(offsetkey) + @"[_\-])(?!.*[_\-\/\\]" + Regex.Escape(offsetkey) + @"[_\-].*[_\-\/\\]" + Regex.Escape(offsetkey) + @"[_\-])";
            string rule2_5 = @"^(?=.*[_\-\/\\]" + Regex.Escape(binkey) + @"[_\-])(?!.*[_\-\/\\]" + Regex.Escape(binkey) + @"[_\-].*[_\-\/\\]" + Regex.Escape(binkey) + @"[_\-])";
            string rule2_6 = @"^(?=.*[_\-\/\\]" + Regex.Escape(readoutkey) + @"[_\-])(?!.*[_\-\/\\]" + Regex.Escape(readoutkey) + @"[_\-].*[_\-\/\\]" + Regex.Escape(readoutkey) + @"[_\-])";
            string rule2_7 = @"^(?=.*[_\-\/\\]" + Regex.Escape(rotkey) + @"[_\-])(?!.*[_\-\/\\]" + Regex.Escape(rotkey) + @"[_\-].*[_\-\/\\]" + Regex.Escape(rotkey) + @"[_\-])";
            string rule3_1 = @"(?:[_\-\\]" + Regex.Escape(epochkey) + @"[_\-])\$\$FLAT_EPOCH\$\$[_\-\\]";
            string rule3_2 = @"(?:[_\-\\]" + Regex.Escape(filterkey) + @"[_\-])\$\$FILTER\$\$[_\-\\]";
            string rule4_1 = @"(?:[_\-\\]" + Regex.Escape(gainkey) + @"[_\-])\$\$GAIN\$\$[_\-\\]";
            string rule4_2 = @"(?:[_\-\\]" + Regex.Escape(offsetkey) + @"[_\-])\$\$OFFSET\$\$[_\-\\]";
            string rule4_3 = @"(?:[_\-\\]" + Regex.Escape(binkey) + @"[_\-])\$\$BINNING\$\$[_\-\\]";
            string rule4_4 = @"(?:[_\-\\]" + Regex.Escape(readoutkey) + @"[_\-])\$\$READOUTMODE\$\$[_\-\\]";
            string rule4_5 = @"(?:[_\-\\]" + Regex.Escape(rotkey) + @"[_\-])\$\$ROTATORANGLE\$\$[_\-\\]";
            string rule5_1 = @"(?:[_\-\\]" + Regex.Escape(epochkey) + @"[_ -])(?=.*\\)(.*?)(?:[_ -\\]" + Regex.Escape(filterkey) + @"[_ -])";
            string rule5_2 = @"(?:[_\-\\]" + Regex.Escape(epochkey) + @"[_ -])(?=.*\\)(.*?)(?:[_ -\\]" + Regex.Escape(gainkey) + @"[_ -])";
            string rule5_3 = @"(?:[_\-\\]" + Regex.Escape(epochkey) + @"[_ -])(?=.*\\)(.*?)(?:[_ -\\]" + Regex.Escape(offsetkey) + @"[_ -])";
            string rule5_4 = @"(?:[_\-\\]" + Regex.Escape(epochkey) + @"[_ -])(?=.*\\)(.*?)(?:[_ -\\]" + Regex.Escape(binkey) + @"[_ -])";
            string rule5_5 = @"(?:[_\-\\]" + Regex.Escape(epochkey) + @"[_ -])(?=.*\\)(.*?)(?:[_ -\\]" + Regex.Escape(readoutkey) + @"[_ -])";
            string rule5_6 = @"(?:[_\-\\]" + Regex.Escape(epochkey) + @"[_ -])(?=.*\\)(.*?)(?:[_ -\\]" + Regex.Escape(rotkey) + @"[_ -])";
            string frames1 = @"^(?:[3-9]|[1-9][0-9]|1[0-9]{2}|200)\s*(?:,\s*(.*)|:\s*(.*))?$";
            string frames2 = @"^(?:[3-9]|[1-9][0-9]|1[0-9]{2}|200)\s*(?::\s*(?:[1-9]|[1-9][0-9]{0,2}|1[0-9]{3}|2000))?\s*(?:,\s*(.*))?$";
            string frames3 = @"^(?:[3-9]|[1-9][0-9]|1[0-9]{2}|200)\s*(?::\s*(?:[1-9]|[1-9][0-9]{0,2}|1[0-9]{3}|2000))?\s*(?:,\s*[A-Za-z]{1,5}\s*=\s*(?:0|[3-9]|[1-9][0-9]|[12][0-9]{2}|200)){0,10}$";
            string cepoch1 = @"^[^0]";
            string cepoch2 = @"^[1-9][0-9]*$";


            // Check the rules
            if (!Regex.IsMatch(currepoch, cepoch1)) {
                return (false, "Current Epoch must not start with a zero");
            }

            if (!Regex.IsMatch(currepoch, cepoch2)) {
                return (false, "Current Epoch must be an integer > 0");
            }

            if (!Regex.IsMatch(nrflats, frames1)) {
                return (false, "The default 'Frames per Flat' must be in range 3-200");
            }

            if (!Regex.IsMatch(nrflats, frames2)) {
                return (false, "The 'Frames per Flat' inter frame delay must be in range 1-2000");
            }

            if (!Regex.IsMatch(nrflats, frames3)) {
                return (false, "The 'Frames per Flat' filter overides malformed");
            }

            if (!Regex.IsMatch(filepattern, rule1_1)) {
                return (false, "FAILED - Rule 1 - Image File Pattern must start with $$IMAGETYPE$$");
            }

            // Check for duplicate keys
            string firstDuplicate = GetFirstDuplicate(epochkey, filterkey, gainkey, offsetkey, binkey, readoutkey, rotkey);
            if (!string.IsNullOrEmpty(firstDuplicate)) {
                return (false, "FAILED - Rule 2 - Duplicate keys defined with the value '" + firstDuplicate + "'");
            }

            if (!Regex.IsMatch(filepattern, rule2_1)) {
                return (false, "FAILED - Rule 2 - The key '" + epochkey + "_' missing or occurs multiple times in the IFP");
            }

            if (!Regex.IsMatch(filepattern, rule2_2)) {
                return (false, "FAILED - Rule 2 - The key '" + filterkey + "_' missing or occurs multiple times in the IFP");
            }

            if (!Regex.IsMatch(filepattern, rule3_1)) {
                return (false, "FAILED - Rule 3 - Epoch key_value pair '" + epochkey + "_$$FLAT_EPOCH$$' not found in the IFP");
            }
            
            if (!Regex.IsMatch(filepattern, rule3_2)) {
                return (false, "FAILED - Rule 3 - Filter key_value pair '" + filterkey + "_$$FILTER$$' not found in the IFP");
            }

            if (!Regex.IsMatch(filepattern, rule5_1)) {
                return (false, "FAILED - Rule 5 - Epoch key '" + epochkey + "_' must be a directory above the Filter key '" + filterkey + "'");
            }

            // Check optional rules
            if (!string.IsNullOrEmpty(gainkey)) {
                if (!Regex.IsMatch(filepattern, rule2_3)) {
                    return (false, "FAILED - Rule 2 - The key '" + gainkey + "_' missing or occurs multiple times in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule4_1)) {
                    return (false, "FAILED - Rule 4 - Gain key_value pair '" + gainkey + "_$$GAIN$$' not found in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule5_2)) {
                    return (false, "FAILED - Rule 5 - Epoch key '" + epochkey + "_' must be a directory above the Gain key '" + gainkey + "'");
                }

            }

            if (!string.IsNullOrEmpty(offsetkey)) {
                if (!Regex.IsMatch(filepattern, rule2_4)) {
                    return (false, "FAILED - Rule 2 - The key '" + offsetkey + "_' missing or occurs multiple times in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule4_2)) {
                    return (false, "FAILED - Rule 4 - Offset key_value pair '" + offsetkey + "_$$OFFSET$$' not found in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule5_3)) {
                    return (false, "FAILED - Rule 5 - Epoch key '" + epochkey + "_' must be a directory above the Offset key '" + offsetkey + "'");
                }
            }

            if (!string.IsNullOrEmpty(binkey)) {
                if (!Regex.IsMatch(filepattern, rule2_5)) {
                    return (false, "FAILED - Rule 2 - The key '" + binkey + "_' missing or occurs multiple times in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule4_3)) {
                    return (false, "FAILED  -Rule 4 - Binning key_value pair '" + binkey + "_$$BINNING$$' not found in the IFP"); 
                }
                if (!Regex.IsMatch(filepattern, rule5_4)) {
                    return (false, "FAILED - Rule 5 - Epoch key '" + epochkey + "_' must be a directory above the Binning key '" + binkey + "'");
                }
            }

            if (!string.IsNullOrEmpty(readoutkey)) {
                if (!Regex.IsMatch(filepattern, rule2_6)) {
                    return (false, "FAILED - Rule 2 - The key '" + readoutkey + "_' missing or occurs multiple times in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule4_4)) {
                    return (false, "FAILED  -Rule 4 - Readout Mode key_value pair '" + readoutkey + "_$$READOUTMODE$$' not found in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule5_5)) {
                    return (false, "FAILED - Rule 5 - Epoch key '" + epochkey + "_' must be a directory above the Readout Mode key '" + readoutkey + "'");
                }
            }

            if (!string.IsNullOrEmpty(rotkey)) {
                if (!Regex.IsMatch(filepattern, rule2_7)) {
                    return (false, "FAILED - Rule 2 - The key '" + rotkey + "_' missing or occurs multiple times in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule4_5)) {
                    return (false, "FAILED - Rule 4 - Rotator Angle key_value pair '" + rotkey + "_$$$ROTATORANGLE$$' not found in the IFP");
                }
                if (!Regex.IsMatch(filepattern, rule5_6)) {
                    return (false, "FAILED - Rule 5 - Epoch key '" + epochkey + "_' must be a directory above the Gain key '" + gainkey + "'");
                }
            }

            return (true,"PASSED - Flat Epoch is configured correctly");
        }


        static string GetFirstDuplicate(params string[] keys) {
            for (int i = 0; i < keys.Length; i++) {
                for (int j = i + 1; j < keys.Length; j++) {
                    if (keys[i] == keys[j]) {
                        return keys[i];
                    }
                }
            }
            return null;
        }

    }

    public class Util {

        public static int LookupExposures(string input, string key) {
            string[] parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int defaultValue = 0;
            int msDelay = 0;

            // Check for possible colon
            string[] defaultParts = parts[0].Trim().Split(':');

            if (defaultParts.Length == 2) {
                defaultValue = int.Parse(defaultParts[0].Trim());
                msDelay = int.Parse(defaultParts[1].Trim());
            } else {
                defaultValue = int.Parse(defaultParts[0].Trim());
            }

            // Store key-value pairs
            Dictionary<string, int> keyValues = new Dictionary<string, int>();

            // Add msDelay to the dictionary
            keyValues["msDelay"] = msDelay;

            // Iterate over remaining parts
            for (int i = 1; i < parts.Length; i++) {
                // Split the part by '=' to get the key and value
                string[] keyValue = parts[i].Trim().Split('=');

                if (keyValue.Length == 2) {
                    string k = keyValue[0].Trim();
                    int v = int.Parse(keyValue[1].Trim());
                    keyValues[k] = v;
                }
            }

            // Return given key value or default if key not found
            if (keyValues.TryGetValue(key, out int value)) {
                return value;
            } else {
                return defaultValue;
            }
        }

        public static TrainedFlatExposureSetting GetTrainedFlatExposureData(Collection<TrainedFlatExposureSetting> trainingData, FilterInfo filter, int gain, int offset, short binX, short binY) {
            int filterPosition = filter.Position;
            if (filterPosition == -1) { return null; }

            return trainingData.FirstOrDefault(setting =>
                    setting.Filter == filterPosition
                    && setting.Binning.X == binX
                    && setting.Binning.Y == binY
                    && setting.Gain == gain
                    && setting.Offset == offset)
                ?? trainingData.FirstOrDefault(setting =>
                    setting.Filter == filterPosition
                    && setting.Binning.X == binX
                    && setting.Binning.Y == binY
                    && setting.Gain == gain)
                ?? trainingData.FirstOrDefault(setting =>
                    setting.Filter == filterPosition
                    && setting.Binning.X == binX
                    && setting.Binning.Y == binY);
        }

    }

}
