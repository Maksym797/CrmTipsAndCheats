using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AuthAndRequests
{
    public class XmlHandler
    {
        private static void Sort(XElement source, bool bSortAttributes = true)
        {
            //Make sure there is a valid source
            if (source == null) throw new ArgumentNullException(nameof(source));

            //Sort attributes if needed
            //if (bSortAttributes)
            //{
            //    List<XAttribute> sortedAttributes = source.Attributes().OrderBy(a => a.ToString()).ToList();
            //    sortedAttributes.ForEach(a => a.Remove());
            //    sortedAttributes.ForEach(source.Add);
            //}

            //Sort the children IF any exist
            List<XElement> sortedChildren = source.Elements()
                .OrderBy(elem => elem.Elements("Dependent").Attributes("key").Any() ? elem.Elements("Dependent").Attributes("key").First().Value.ToString() : string.Empty)
                .ThenBy(elem => elem.Elements("Required").Attributes("key").Any() ? elem.Elements("Required").Attributes("key").First().Value.ToString() : string.Empty)
                .ThenBy(elem => elem.Elements("Required").Attributes("displayName").Any() ? elem.Elements("Required").Attributes("displayName").First().Value.ToString() : string.Empty)
                .ThenBy(elem => elem.Elements("Dependent").Attributes("id").Any() ? elem.Elements("Dependent").Attributes("id").First().Value.ToString() : string.Empty)
                .ThenBy(elem => elem.Elements("Required").Attributes("schemaName").Any() ? elem.Elements("Required").Attributes("schemaName").First().Value.ToString() : string.Empty)
                .ToList();

            if (source.HasElements)
            {
                source.RemoveNodes();
                sortedChildren.ForEach(c => Sort(c));
                sortedChildren.ForEach(source.Add);
            }
        }
    }
}
