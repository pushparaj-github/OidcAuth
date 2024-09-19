using System;
using System.Collections.Generic;
using System.IO;

namespace OidcAuth
{
    public class IniFileReader
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data = new Dictionary<string, Dictionary<string, string>>();

        public IniFileReader(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file '{filePath}' does not exist.");

            Load(filePath);
        }

        private void Load(string filePath)
        {
            string currentSection = null;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                    continue;

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    if (!_data.ContainsKey(currentSection))
                    {
                        _data[currentSection] = new Dictionary<string, string>();
                    }
                }
                else if (currentSection != null)
                {
                    var keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        _data[currentSection][keyValue[0].Trim()] = keyValue[1].Trim();
                    }
                }
            }
        }

        public string GetValue(string section, string key, string defaultValue = null)
        {
            if (_data.ContainsKey(section) && _data[section].ContainsKey(key))
            {
                return _data[section][key];
            }
            return defaultValue;
        }
    }
}
