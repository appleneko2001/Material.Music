using System.Collections.Generic;
using JetBrains.Annotations;
using Material.Music.Online.Interfaces;

namespace Material.Music.Online
{
    public class ApiManager
    {
        private static ApiManager _instance;
        public static ApiManager Instance => _instance;

        private List<ISubtitleProvider> _subtitleProviders;
        public IReadOnlyCollection<ISubtitleProvider> SubtitleProviders => _subtitleProviders;

        static ApiManager()
        {
            _instance = new ApiManager();
        }

        public ApiManager()
        {
            _subtitleProviders = new List<ISubtitleProvider>();
        }

        public bool RegisterSubtitleApi(ISubtitleProvider api)
        {
            _subtitleProviders.Add(api);
            return _subtitleProviders.Contains(api);
        }
    }
}