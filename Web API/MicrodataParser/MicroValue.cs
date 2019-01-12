namespace MicrodataParser
{
    public interface IMicroValue
    {
        object ToStringOrDictionary();        
    }

    public class MicroStringValue: IMicroValue
    {
        public MicroStringValue(string value)
        {
            Value = value;
        }

        string Value { get; set;}
        public object ToStringOrDictionary()
        {
            return Value;
        }
    }

    public class MicroObjectValue: IMicroValue
    {
        public MicroObjectValue(MicroObject value)
        {
            Value = value;
        } 

        MicroObject Value { get; set; }

        public object ToStringOrDictionary()
        {
            return Value.ToDictionary();
        }

    }
}
