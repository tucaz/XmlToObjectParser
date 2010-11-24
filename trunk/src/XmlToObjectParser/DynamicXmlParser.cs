using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace XmlToObjectParser
{
    public class DynamicXmlParser
    {
        public static dynamic ParseFromXml(string xml)
        {
            IDictionary<string, object> parsedObject = new ExpandoObject();

            var rootElement = XElement.Parse(xml);

            var root = CreateChildElement(rootElement);

            parsedObject.Add(rootElement.Name.LocalName, root);

            return parsedObject;
        }

        private static dynamic CreateChildElement(XElement parent)
        {
            if (parent.Attributes().Count() == 0 && parent.Elements().Count() == 0)
                return null;

            IDictionary<string, object> child = new ExpandoObject();

            parent.Attributes().ToList().ForEach(attr =>
            {
                child.Add(attr.Name.LocalName, attr.Value);

                if (!child.ContainsKey("NodeName"))
                    child.Add("NodeName", attr.Parent.Name.LocalName);
            });

            parent.Elements().ToList().ForEach(childElement =>
            {
                var grandChild = CreateChildElement(childElement);

                if (grandChild != null)
                {
                    string nodeName = grandChild.NodeName;
                    if (child.ContainsKey(nodeName) && child[nodeName].GetType() != typeof(List<dynamic>))
                    {
                        var firstValue = child[nodeName];
                        child[nodeName] = new List<dynamic>();
                        ((dynamic)child[nodeName]).Add(firstValue);
                        ((dynamic)child[nodeName]).Add(grandChild);
                    }
                    else if (child.ContainsKey(nodeName) && child[nodeName].GetType() == typeof(List<dynamic>))
                    {
                        ((dynamic)child[nodeName]).Add(grandChild);
                    }
                    else
                    {
                        child.Add(childElement.Name.LocalName, CreateChildElement(childElement));
                        if (!child.ContainsKey("NodeName"))
                            child.Add("NodeName", parent.Name.LocalName);
                    }
                }
                else
                {
                    child.Add(childElement.Name.LocalName, childElement.Value);
                    if (!child.ContainsKey("NodeName"))
                        child.Add("NodeName", parent.Name.LocalName);
                }
            });

            return child;
        }
    }
}
