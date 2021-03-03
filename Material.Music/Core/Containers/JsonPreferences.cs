using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Material.Music.Core.Containers
{
    public class JsonPreferences
    {
        private string _fullPath;

        public static JsonWriterOptions WriterOptions = new JsonWriterOptions() {Indented = true};
        
        public Dictionary<string, JsonPreferenceValue> Preferences;
        
        public static string AppdataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "NekoPlayer");
        
        public static string OldFilesPath => Path.Combine(AppdataPath, "FallbackFiles");

        public JsonPreferences()
        {
            Preferences = new Dictionary<string, JsonPreferenceValue>();
        }
        
        public JsonPreferences(string fileName = "Preferences.json")
        {
            _fullPath = Path.Combine(AppdataPath, fileName);
            if (File.Exists(_fullPath))
            {
                var status = JsonSerializer.Deserialize<JsonPreferences>(File.ReadAllText(_fullPath));
                if (status.Preferences != null)
                    Preferences = status.Preferences;
            }
            else
            {
                Preferences = new Dictionary<string, JsonPreferenceValue>();
                ApplyChanges();
            }
        }

        public void Load()
        {
            var reader = new Utf8JsonReader(File.ReadAllBytes(_fullPath));
           
        }

        public void ApplyChanges()
        {
            using (var stream = File.Open(_fullPath, FileMode.Open))
            {
                using (var writer = new Utf8JsonWriter(stream, WriterOptions))
                {
                    
                }
            }
        }
    }
}