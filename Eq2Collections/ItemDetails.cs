using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eq2Collections
{
    [XmlRoot("item_list")]
    public class ItemDetails
    {
        [XmlAttribute("limit")]
        public string limit { get; set; }
        [XmlAttribute("returned")]
        public string returned { get; set; }
        [XmlElement("item")]
        public ItemDetail item;
    }

    public class ItemDetail
    {
        [XmlAttribute("displayname_lower")]
        public string displayname_lower { get; set; }
        [XmlAttribute("displayname")]
        public string displayname { get; set; }
        [XmlAttribute("iconid")]
        public string iconid { get; set; }
        [XmlElement("flags")]
        public Flags flags;
    }

    public class Flags
    {
        [XmlElement("heirloom")]
        public Heirloom heirloom;
        [XmlElement("lore")]
        public Lore lore;
        [XmlElement("notrade")]
        public NoTrade notrade;
    }

    public class Heirloom
    {
        [XmlAttribute("value")]
        public string value { get; set; }

        public override string ToString()
        {
            return value;
        }
    }

    public class Lore
    {
        [XmlAttribute("value")]
        public string value { get; set; }

        public override string ToString()
        {
            return value;
        }
    }

    public class NoTrade
    {
        [XmlAttribute("value")]
        public string value { get; set; }

        public override string ToString()
        {
            return value;
        }
    }
}
