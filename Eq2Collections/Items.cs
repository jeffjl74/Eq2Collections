using System.Collections.Generic;
using System.Xml.Serialization;

namespace Eq2Collections
{
    [XmlRoot("item_list")]
    public class Items
    {
        [XmlAttribute("limit")]
        public string limit { get; set; }
        [XmlAttribute("returned")]
        public string returned { get; set; }
        [XmlElement("item")]
        public List<Item> item;
    }
}
