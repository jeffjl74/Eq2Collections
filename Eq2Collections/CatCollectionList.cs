using System.Collections.Generic;
using System.Xml.Serialization;

namespace Eq2Collections
{
    //
    //category list
    //
    [XmlRoot("collection_list")]
    public class CatCollectionList
    {
        [XmlAttribute("limit")]
        public string limit { get; set; }
        [XmlAttribute("returned")]
        public string returned { get; set; }
        [XmlElement("collection")]
        public List<CatCollection> collection { get; set; }
    }

    public class CatCollection
    {
        [XmlAttribute("category")]
        public string category { get; set; }
        [XmlAttribute("level")]
        public string level { get; set; }
        [XmlAttribute("id")]
        public string id { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1,3})", category, level);
        }

        public string ByLevel()
        {
            return string.Format("{0,3})", level);
        }

    }

    public class CategoryList
    {
        [XmlElement("category")]
        public List<Category> category { get; set; }
    }

    public class Category
    {
        //[XmlElement("category")]
        [XmlText]
        public string category { get; set; }

        public override string ToString()
        {
            return category.TrimStart('"').TrimEnd('"');
        }
    }
}
