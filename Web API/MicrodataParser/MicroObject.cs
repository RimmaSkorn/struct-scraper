using System.Collections.Generic;
using System.Linq;

namespace MicrodataParser
{
    public class MicroObject
    {
        public MicroObject()
        {
            Types = new List<string>();
        }

        public IList<string> Types { get; set;}
        public string ID { get; set; }

        public IList<MicroProperty> Properties;

        public IDictionary<string, object> ToDictionary()
        {
            IDictionary<string, object> d = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                d.Add(itemIdString, ID);
            }
            if (Types.Count == 1)
            {
                d.Add(itemTypeString, Types[0]);
            }
            else if (Types.Count > 1)
            {
                d.Add(itemTypeString, Types);
            }
            var propNames = (from prop in Properties
                             select prop.Name).Distinct();
            foreach (string name in propNames)
            {
                var props = (from prop in Properties
                             where prop.Name == name
                             select prop);
                if (props.Count() == 1)
                {
                    d.Add(name, props.FirstOrDefault().Value.ToStringOrDictionary());
                }
                else
                {
                    var values = (from prop in props
                                  select prop.Value.ToStringOrDictionary()).ToList();
                    d.Add(name, values);
                }
            }

            return d;

        }

        private const string itemIdString = "itemId";
        private const string itemTypeString = "itemType";

    }
}
