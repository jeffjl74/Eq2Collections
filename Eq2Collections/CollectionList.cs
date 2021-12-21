using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Eq2Collections.Form1;

namespace Eq2Collections
{
    //
    //collection root
    //
    [XmlRoot("collection_list")]
    public class CollectionList
    {
        [XmlAttribute("limit")]
        public string limit { get; set; }
        [XmlAttribute("returned")]
        public string returned { get; set; }
        [XmlElement("collection")]
        public List<Collection> collection { get; set; }
    }

    public class Collection : IComparable<Collection>
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("category")]
        public string category { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("level")]
        public string level { get; set; }
        [XmlElement("reference_list")]
        public ReferenceList referenceList { get; set; }
        [XmlElement("reward_list")]
        public RewardList rewardList { get; set; }

        public string parent_id = string.Empty;
        public List<Collection> children = new List<Collection>();
        public bool have;

        public override string ToString()
        {
            return name;
        }

        public bool HaveAllItems()
        {
            bool all = true;
            foreach (Reference refr in referenceList.reference)
            {
                all &= refr.have;
            }
            return all;
        }

        public bool HaveAnyItems()
        {
            bool any = false;
            foreach (Reference refr in referenceList.reference)
            {
                if (refr.have)
                {
                    any = true;
                    break;
                }
            }
            if (!any)
            {
                //look at children
                foreach (Collection c in children)
                {
                    any = c.HaveAnyItems();
                    if (any)
                        break;
                }
            }
            return any;
        }

        public void SetHaveNoItems()
        {
            foreach (Reference refr in referenceList.reference)
            {
                refr.have = false;
            }
        }

        public int CompareTo(Collection other)
        {
            return CompareStringsWithNumbers(this.name, other.name);
        }
    }

    public class ReferenceList
    {
        [XmlElement("reference")]
        public List<Reference> reference { get; set; }

        public bool sorted = false;
    }

    public class Reference : IComparable<Reference>
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("icon")]
        public string icon { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }

        public bool have;
        public bool isReward = false;
        public ItemDetail itemDetail = null;
        public Image image = null;
        public string collectionId = null;

        public override string ToString()
        {
            return name;
        }

        public int CompareTo(Reference other)
        {
            return CompareStringsWithNumbers(this.name, other.name);
        }
    }

    public class RewardList
    {
        [XmlElement("reward")]
        public Reward reward { get; set; }
    }

    public class Reward
    {
        [XmlAttribute("coin_max")]
        public string coin_max { get; set; }
        [XmlAttribute("coin_min")]
        public string coin_min { get; set; }
        [XmlElement("item_list")]
        public ItemList itemList { get; set; }
        [XmlElement("selected_item_list")]
        public SelectedItemList selectedItemList { get; set; }
    }

    public class ItemList
    {
        [XmlElement("item")]
        public List<Item> item { get; set; }
    }

    public class Item
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("quantity")]
        public string quantity { get; set; }

        public ItemDetail itemDetail = null;
        public Image image = null;
        public string collectionId = null;
    }

    public class SelectedItemList
    {
        [XmlElement("selected_item")]
        public List<Item> selectedItem { get; set; }
    }

    public class SelectedItem
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("quantity")]
        public string quantity { get; set; }
    }

}
