namespace Material.Music.Core.Containers
{
    public enum JsonPreferenceValueType
    {
        String,
        Integer,
        Boolean,
    }
    
    public class JsonPreferenceValue
    {
        public JsonPreferenceValueType Type;
        public object Value;
    }
}