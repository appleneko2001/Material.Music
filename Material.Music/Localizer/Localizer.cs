using Avalonia;
using Avalonia.Platform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Material.Music.Localizer
{
    public class Localizer : INotifyPropertyChanged
    {
        private const string IndexerName = "Item";
        private const string IndexerArrayName = "Item[]";
        private Dictionary<string, string> _strings = null;

        public bool LoadLanguage(string language)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            _strings = new Dictionary<string, string>(comparer);
            
            Language = language;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            var uri = new Uri($"avares://Material.Music/Assets/International/{language}.json");
            if (assets.Exists(uri)) {
                using (var sr = new StreamReader(assets.Open(uri), Encoding.UTF8))
                {
                    var COLLECTION = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                    foreach (var variable in COLLECTION)
                    {
                        _strings.Add(variable.Key, variable.Value);
                    }
                }
                Invalidate();

                return true;
            }
            return false;
        }

        public string Language { get; private set; }

        public string this[string key]
        {
            get
            {
                string res;
                if (_strings != null && _strings.TryGetValue(key, out res))
                    return res.Replace("\\n", "\n");

                return $"{Language}:{key}";
            }
        }

        private static Localizer _instance;
        public static Localizer Instance
        {
            get
            {
                if(_instance is null)
                    _instance = new Localizer();
                return _instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
        }
    }
}