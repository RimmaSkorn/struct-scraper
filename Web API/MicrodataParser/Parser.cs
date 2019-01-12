using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace MicrodataParser
{
    public class Parser
    {
        private const string itemscopeString = "itemscope";
        private const string itempropString = "itemprop";
        private const string itemtypeString = "itemtype";
        private const string itemidString = "itemid";
        private const string itemrefString = "itemref";

        private const string schemaString = "//schema.org/";

        private const string scriptString = "script";
        private const string styleString = "style";

        private const string metaString = "meta";
        private const string mediaString = "audio,embed,iframe,img,source,track,video";
        private const string linksString = "a,area,link";
        private const string objectString = "object";
        private const string timeString = "time";

        private const string contentString = "content";
        private const string srcString = "src";
        private const string hrefString = "href";
        private const string dataString = "data";
        private const string valueString = "value";
        private const string datetimeString = "datetime";

        public static IList<MicroObject> Parse(string html, string schemaType)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            //remove scripts & style
            doc.DocumentNode.Descendants()
                .Where(n => n.Name.Equals(scriptString, StringComparison.OrdinalIgnoreCase) || n.Name.Equals(styleString, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(n => n.Remove());

            // top level nodes with type = schemaType 
            var topLevelMicroNodes = doc.DocumentNode.Descendants()
                                   .Where(n => n.Attributes.Contains(itemscopeString) && !n.Attributes.Contains(itempropString) && NodeIsOfType(n,schemaType)); 

              
            List<MicroObject> mos = new List<MicroObject>();

            foreach (var microNode in topLevelMicroNodes)
                mos.Add(GetMicroObject(doc, microNode));

            return mos;
        }

        private static bool NodeIsOfType(HtmlNode node, string schemaType)
        {
            if (String.IsNullOrEmpty(schemaType))
                return true;

            if (!node.Attributes.Contains(itemtypeString))
                return false;

            IList<string> types = node.Attributes[itemtypeString].Value.Split(' ').ToList();

            bool result = false;
            foreach (string t in types)
            {
                string type = t.Trim();
                if (type.EndsWith(schemaString + schemaType, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private static MicroObject GetMicroObject(HtmlDocument doc, HtmlNode microNode)
        {
            MicroObject mo = new MicroObject();

            if (microNode.Attributes.Contains(itemtypeString))
                microNode.Attributes[itemtypeString].Value.Split(' ').ToList().ForEach(it => mo.Types.Add(it));

            if (microNode.Attributes.Contains(itemidString))
                mo.ID = microNode.Attributes[itemidString].Value;

            mo.Properties = GetProperties(doc, GetPropNodes(doc, microNode));

            return mo;
        }

        private static IList<MicroProperty> GetProperties(HtmlDocument doc, IList<HtmlNode> propNodes)
        {
            IList<MicroProperty> properties = new List<MicroProperty>();

            foreach (var propNode in propNodes)
            {
                string propNodeName = propNode.Name.ToLower();
                string propName = propNode.Attributes[itempropString].Value;

                MicroProperty prop = new MicroProperty();
                prop.Name = propName;


                if (propNode.Attributes.Contains(itemscopeString))
                {
                    prop.Value = new MicroObjectValue(GetMicroObject(doc, propNode));
                }
                else if ((propNodeName == metaString) && (propNode.Attributes[contentString] != null))
                    prop.Value = new MicroStringValue(propNode.Attributes[contentString].Value);
                else if ((mediaString.Contains(propNodeName + ",")) && (propNode.Attributes[srcString] != null))
                    prop.Value = new MicroStringValue(propNode.Attributes[srcString].Value);
                else if ((linksString.Contains(propNodeName + ",")) && (propNode.Attributes[hrefString] != null))
                    prop.Value = new MicroStringValue(propNode.Attributes[hrefString].Value);
                else if ((propNodeName == objectString) && (propNode.Attributes[dataString] != null))
                    prop.Value = new MicroStringValue(propNode.Attributes[dataString].Value);
                else if ((propNodeName == dataString) && (propNode.Attributes[valueString] != null))
                    prop.Value = new MicroStringValue(propNode.Attributes[valueString].Value);
                else if ((propNodeName == timeString) && (propNode.Attributes[datetimeString] != null))
                    prop.Value = new MicroStringValue(propNode.Attributes[datetimeString].Value);

                // This is not from the Specifications, but some sites are using it.
                else if (propNode.Attributes.Contains(contentString))
                    prop.Value = new MicroStringValue(HtmlEntity.DeEntitize(propNode.Attributes[contentString].Value).Trim());
                else
                    prop.Value = new MicroStringValue(HtmlEntity.DeEntitize(propNode.InnerText).Trim());

                properties.Add(prop);

            }

            return properties;
        }

        private static IList<HtmlNode> GetPropNodes(HtmlDocument doc, HtmlNode microNode)
        {
            Queue<HtmlNode> memory = new Queue<HtmlNode>();
            IList<HtmlNode> results = new List<HtmlNode>();
            Queue<HtmlNode> pending = new Queue<HtmlNode>();

            memory.Enqueue(microNode);
            foreach (var children in microNode.ChildNodes)
                pending.Enqueue(children);

            if (microNode.Attributes.Contains(itemrefString))
                foreach (string itemref in microNode.Attributes[itemrefString].Value.Split(' '))
                    pending.Enqueue(doc.DocumentNode.SelectSingleNode("//" + itemref));

            while (pending.Count > 0)
            {
                var currentItem = pending.Dequeue();

                if (currentItem != null)
                {
                    // If the node is already added, skip it
                    if (memory.Contains(currentItem))
                        continue;
                    memory.Enqueue(currentItem);

                    // If the node is not an ItemScope, enqueue it
                    if (!currentItem.Attributes.Contains(itemscopeString))
                        foreach (var children in currentItem.ChildNodes)
                            pending.Enqueue(children);

                    // If the node is an itemprop, it's... an itemprop
                    if (currentItem.Attributes.Contains(itempropString))
                        results.Add(currentItem);

                }

            }

            return results;
        }

    }
}
