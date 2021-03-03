namespace Material.Music.Core.Containers
{
    public class JsonPreferenceEntry
    {
        public JsonPreferenceEntry(JsonPreferences parent, string entryName, JsonPreferenceValueType type,
            object defaultValue = null)
        {
            _parent = parent;
            Name = entryName;
            Type = type;
        }

        public void SetValue(object value)
        {
            
        }
        
        public object GetValue()
        {
            return null;
        }
        
        public string Name { get; private set; }

        public JsonPreferenceValueType Type { get; private set; }

        private JsonPreferences _parent;
    }
}