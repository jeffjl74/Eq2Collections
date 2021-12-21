using System.Collections.Generic;
using System.Xml.Serialization;

namespace Eq2Collections
{
    [XmlRoot("world_list")]
    public class Worlds
    {
        [XmlAttribute("limit")]
        public string limit { get; set; }
        [XmlAttribute("returned")]
        public string returned { get; set; }
        [XmlElement("world")]
        public List<World> world;
    }

    public class World
    {
        [XmlAttribute("status")]
        public string status { get; set; }
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("language")]
        public string language { get; set; }
        [XmlAttribute("ts")]
        public string ts { get; set; }
        [XmlAttribute("last_update")]
        public string last_update { get; set; }
        [XmlAttribute("name_lower")]
        public string name_lower { get; set; }
        [XmlAttribute("id")]
        public string id { get; set; }
    }
}
