using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Material.Music.Core.Containers
{
    public abstract class JsonDataContainer<TSave> where TSave : class
    {
        private TSave _mInstance; 

        public static string AppdataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "NekoPlayer");
        public static string OldFilesPath => Path.Combine(AppdataPath, "FallbackFiles");

        [JsonIgnore]
        public abstract string JsonSaveSubdir { get; }

        [JsonIgnore]
        public abstract string JsonSaveFilename { get; }

        public void SetTargetSaves(TSave instance) => _mInstance = instance;

        public void SaveStatusAsJson()
        {
            var inst = _mInstance as JsonDataContainer<TSave>;
            var subdir = Path.Combine(AppdataPath, inst.JsonSaveSubdir);
            try
            {
                CreateIfNotExistDir(inst.JsonSaveSubdir);

                var filePath = Path.Combine(subdir, inst.JsonSaveFilename);
                if (File.Exists(filePath))
                {
                    var fallbackFile = Path.Combine(OldFilesPath, inst.JsonSaveSubdir, inst.JsonSaveFilename);
                    if (File.Exists(fallbackFile))
                        File.Delete(fallbackFile);
                    File.Move(filePath, fallbackFile);
                }

                var serializedData = JsonSerializer.Serialize(_mInstance, options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });
                File.WriteAllText(filePath, serializedData);
            }
            catch(Exception e)
            {

            }
        } 

        public async Task LoadStatusFromJsonAsync()
        {
            var inst = _mInstance as JsonDataContainer<TSave>;
            var subdir = Path.Combine(AppdataPath, inst.JsonSaveSubdir);
            try
            {
                var filePath = Path.Combine(subdir, inst.JsonSaveFilename);
                var fallbackFile = Path.Combine(OldFilesPath, inst.JsonSaveSubdir, inst.JsonSaveFilename);
                while(true)
                {
                    if (!File.Exists(filePath))
                    {
                        if (File.Exists(fallbackFile))
                        {
                            File.Copy(fallbackFile, filePath);
                            continue;
                        }
                    }
                    else
                    {
                        var data = await File.ReadAllTextAsync(filePath);
                        var deserialized = JsonSerializer.Deserialize<TSave>(data, options: new JsonSerializerOptions()
                        {
                            
                        });// JsonConvert.DeserializeObject<TSave>(data);
                        SetFields(deserialized);
                    }
                    break;
                }
            }
            catch(Exception e)
            {

            }
        }

        private void SetFields(object deserialized)
        {
            var inst = _mInstance as JsonDataContainer<TSave>;
            var fields = inst.GetType().GetProperties();
            var saves = deserialized as TSave;
            var savesFields = saves.GetType().GetProperties();
            var availableFields = new Dictionary<PropertyInfo, object>();
            foreach(var field in savesFields)
            {
                var data = field.GetValue(saves);
                if (data != null)
                    availableFields.Add(field, data);
            }

            foreach(var field in fields)
            {
                if (availableFields.ContainsKey(field) && field.CanWrite)
                {
                    var value = availableFields[field];
                    field.SetValue(inst, value);
                }
            }
        }

        private void CreateIfNotExistDir(string subdir)
        {
            var final = Path.Combine(AppdataPath, JsonSaveSubdir);
            var fallback = Path.Combine(OldFilesPath, JsonSaveSubdir);
            if (!Directory.Exists(final))
                Directory.CreateDirectory(final);
            if (!Directory.Exists(fallback))
                Directory.CreateDirectory(fallback);
        }
    } 
}
