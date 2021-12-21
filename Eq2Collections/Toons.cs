using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eq2Collections
{
    [XmlRoot("character_list")]
    public class Toons
    {
        [XmlAttribute("limit")]
        public string limit { get; set; }
        [XmlAttribute("returned")]
        public string returned { get; set; }
        [XmlElement("character")]
        public Character character;
    }

    public class Character
    {
        [XmlAttribute("id")]
        public string id { get; set; }
    }
}
