using System.Collections.Generic;
using System.Xml.Serialization;

namespace Eq2Collections
{
    //
    //character_misc root
    //
    [XmlRoot("character_misc_list")]
    public class CharacterMiscList
    {
        [XmlAttribute("limit")]
        public string limit { get; set; }
        [XmlAttribute("returned")]
        public string returned { get; set; }
        [XmlElement("character_misc")]
        public CharacterMisc characterMisc { get; set; }
    }

    public class CharacterMisc
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlElement("collection_list")]
        public CharMiscColList charMiscColList { get; set; }
    }

    public class CharMiscColList
    {
        [XmlElement("collection")]
        public List<CharMiscCollection> charMiscCollection { get; set; }
    }

    public class CharMiscCollection
    {
        [XmlAttribute("crc")]
        public string crc { get; set; }
        [XmlElement("item_list")]
        public CharMiscItemList charMiscItemList { get; set; }

        public override string ToString()
        {
            return crc;
        }
    }

    public class CharMiscItemList
    {
        [XmlElement("item")]
        public List<CharMiscItem> charMiscItem { get; set; }
    }

    public class CharMiscItem
    {
        [XmlAttribute("crc")]
        public string crc { get; set; }

        public override string ToString()
        {
            return crc;
        }
    }
}
