using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO.Compression;
using System.Diagnostics;
using Eq2Collections.Properties;
using System.Configuration;

namespace Eq2Collections
{
    public partial class Form1 : Form
    {
        static HttpClientHandler handler;
        static HttpClient client;
        CollectionList collectionList;                      //created from XML returned from daybreak, collections for a category
        CharacterMiscList charMiscList;                     //created from XML returned from daybreak, collections for a toon
        CatCollectionList catCollectionList;                //created from XML returned from daybreak, category names with duplicates
        List<CatCollection> consolodatedCats;               //no-duplicates category names
        Dictionary<string, List<Collection>> categories;    //game collection heirarchies grouped by category name
        SortedList<string, List<Collection>> sortedCats;    //sorted (name or level) game collection heirarchies
        Dictionary<string, Reference> flatItems;            //all game collection items by ID
        Dictionary<string, Item> flatRewards;               //all game collection rewards by ID
        Dictionary<string, Reference> lookupItems;          //all game collection items by collectionID-itemID, for toon list matching
        Dictionary<string, CatCollection> catMap;           //all game collections by ID
        bool enableTree = false;                            //disallow tree checkbox changes by the user
        bool enableList = false;                            //disallow list checkbox changes by the user
        Worlds worldList = new Worlds();
        bool needHeirarchy = true;                          //only query the census for categories/collections once
        int toonColCount = 0;                               //how many collected items does a toon have
        CancellationTokenSource cts;                        //cancel previous tree click if get a new one
        TreeNode clickedNode;
        List<Reference> wantList;
        List<FoundItems> foundItems;
        List<TreeNode> foundCollections;
        int findNextItemIndex = -1;
        int findNextCollectionIndex = -1;
        FindForm findItemForm = null;
        FindForm findCollectionForm = null;
        List<TreeNode> finishedNodes = new List<TreeNode>();
        string baseUrl = @"https://census.daybreakgames.com/";
        Version remoteVersion;
        const string githubProject = "Eq2Collections";
        const string githubOwner = "jeffjl74";
        string iconPath = string.Empty;
        const string iconFolder = "UI\\Default\\images\\icons";
        const int iconsPerDDS = 36;
        const int iconSize = 42;
        bool initializing = true;
        Dictionary<int, DDSImage> ddsImages = new Dictionary<int, DDSImage>();

        internal class FoundItems
        {
            public TreeNode treeNode;
            public string itemName;
            public int itemIndex;
        }

        //debug: set non-zero to speed-up/truncate fetch for debug by only fetching that many collections
        // 0 to fetch the entire list
        const int maxCollections = 0;

        public Form1()
        {
            InitializeComponent();

            NewHttp();

            // add the service ID
            // note that without a valid service ID, the number of census queries is strictly limited
            // to far less than this utility needs
            baseUrl = baseUrl + ServiceId.service_id + "/";

            // if there is a previous version file laying around, remove it
            string exeFullName = Application.ExecutablePath;
            string oldName = exeFullName + ".old";
            if (File.Exists(oldName))
                File.Delete(oldName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (!File.Exists(configPath))
            {
                //Existing user config does not exist, so load settings from previous assembly
                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.Save();
            }

            if (!Properties.Settings.Default.WindowSize.Equals(new Size(0, 0)))
                this.Size = Properties.Settings.Default.WindowSize;
            if (Properties.Settings.Default.WindowLocation != null)
            {
                Point clientPt = Properties.Settings.Default.WindowLocation;
                //make sure it fits on screen in case the parent has moved
                if (clientPt.X < 0)
                    clientPt.X = 0;
                if (clientPt.X + this.Size.Width > SystemInformation.VirtualScreen.Right)
                    clientPt.X = SystemInformation.VirtualScreen.Right - this.Size.Width;
                if (clientPt.Y < 0)
                    clientPt.Y = 0;
                if (clientPt.Y + this.Size.Height > SystemInformation.WorkingArea.Bottom)
                    clientPt.Y = SystemInformation.WorkingArea.Bottom - this.Size.Height;
                this.Location = clientPt;
            }
            if (Properties.Settings.Default.SplitterLocation != 0)
                splitContainer1.SplitterDistance = Properties.Settings.Default.SplitterLocation;
            if (Properties.Settings.Default.Player != null)
                textBoxChar.Text = Properties.Settings.Default.Player;
            checkBoxShowIcons.Checked = Properties.Settings.Default.ShowIcons;
            checkBoxLevelSort.Checked = Properties.Settings.Default.SortByLevel;

            if (!string.IsNullOrEmpty(Settings.Default.GameFolder))
            {
                iconPath = Path.Combine(Settings.Default.GameFolder, iconFolder);
                checkBoxGameIcons.Checked = Settings.Default.UseGameIcons;
            }

            ImageList treeImageList = new ImageList();
            treeImageList.Images.Add(Properties.Resources.Folder6);
            treeImageList.Images.Add(Properties.Resources.FolderOpen2);
            treeView1.ImageList = treeImageList;
            treeView2.ImageList = treeImageList;

            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 1000;
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(checkBoxShowIcons, "Check to fetch item icons\n(takes time if using census icons)");
            toolTip.SetToolTip(textBoxChar, "Fetch collection status for this character");
            toolTip.SetToolTip(comboBoxWorlds, "World for the character name");
            toolTip.SetToolTip(buttonChar, "Get the collections for the given character and world");
            toolTip.SetToolTip(checkBoxLevelSort, "Check to sort by level. Uncheck to sort alphabetically.");
            toolTip.SetToolTip(checkBoxHide, "Check to hide completed categories");
            //toolTip.SetToolTip(treeView1, "Top level node is just the category.\nChild nodes contain collections.");

            toolStripStatusLabelItemCount.Visible = false;
            initializing = false;
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            await GetWorlds();
            if (worldList != null)
            {
                //don't need dead worlds
                DateTime today = DateTime.Now;
                TimeSpan span = new TimeSpan(30, 0, 0, 0); //30 days
                if (worldList.world == null)
                    FlexibleMessageBox.Show(this, "Census did not return the list of worlds.");
                else
                {
                    try
                    {
                        foreach (World w in worldList.world)
                        {
                            DateTime updated = UnixTimeStampToDateTime(w.last_update);
                            if (today - updated <= span)
                            {
                                comboBoxWorlds.Items.Add(w.name);
                            }
                        }
                        if (string.IsNullOrEmpty(Properties.Settings.Default.World))
                            comboBoxWorlds.SelectedIndex = 0;
                        else
                            comboBoxWorlds.SelectedItem = Properties.Settings.Default.World;
                    }
                    catch
                    {
                        FlexibleMessageBox.Show(this, "There was a problem getting the world list from the census.\nRestart EQ2Collections to retry.");
                        Properties.Settings.Default.World = string.Empty;
                        Properties.Settings.Default.Save();
                    }
                }
            }

            if(Properties.Settings.Default.SortByLevel)
                GetGameCollections();   //the check-changed call back will get the collections

            listView1.Groups.Add(new ListViewGroup("Collection", HorizontalAlignment.Left));
            listView1.Groups.Add(new ListViewGroup("Reward", HorizontalAlignment.Left));
            listView1.Groups.Add(new ListViewGroup("Selected Reward", HorizontalAlignment.Left));
            listView1.ShowGroups = true;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
                Properties.Settings.Default.SplitterLocation = splitContainer1.SplitterDistance;
            }
            Properties.Settings.Default.World = comboBoxWorlds.Text;
            Properties.Settings.Default.Player = textBoxChar.Text;
            Properties.Settings.Default.ShowIcons = checkBoxShowIcons.Checked;
            Properties.Settings.Default.SortByLevel = checkBoxLevelSort.Checked;
            Properties.Settings.Default.Save();
        }

        #region HTTP

        private void NewHttp()
        {
            handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }
            client = new HttpClient(handler);
            client.Timeout = new TimeSpan(0, 0, 8);
        }

        public async Task GetCollection(CatCollection cat)
        {
            try
            {
                string esc = Uri.EscapeUriString(cat.category);
                var requestUri = baseUrl + @"xml/get/eq2/collection/?&c:limit=750&category=" + esc + @"&level=" + cat.level;
                //UseWaitCursor = true;
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var serializer = new XmlSerializer(typeof(CollectionList));
                        collectionList = (CollectionList)serializer.Deserialize(stream);
                    }
                }
                else
                {
                    FlexibleMessageBox.Show(this,"GetCollections Response: " + response.ReasonPhrase, "Error");
                }
                //UseWaitCursor = false;
            }
            catch(Exception gcolx)
            {
                FlexibleMessageBox.Show(this,"Get Collection exception: " + gcolx.Message, "Error");
            }
        }

        public async Task GetCategories()
        {
            try
            {
                var requestUri = baseUrl + @"xml/get/eq2/collection/?c:show=category,level&c:limit=3000";
                //UseWaitCursor = true;
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var serializer = new XmlSerializer(typeof(CatCollectionList));
                        catCollectionList = (CatCollectionList)serializer.Deserialize(stream);
                    }
                }
                else
                {
                    FlexibleMessageBox.Show(this,"GetCategories Response: " + response.ReasonPhrase, "Error");
                }
                //UseWaitCursor = false;
            }
            catch(Exception gcx)
            {
                FlexibleMessageBox.Show(this,"Get categories exception: " + gcx.Message, "Error");
            }
        }

        public async Task<string> GetCharId()
        {
            string id = string.Empty;
            string boxToon = textBoxChar.Text;
            string toon = char.ToUpper(boxToon[0]) + boxToon.Substring(1);
            string world = Uri.EscapeDataString(comboBoxWorlds.Text);
            if (!string.IsNullOrEmpty(toon) && !string.IsNullOrEmpty(world))
            {
                try
                {
                    var requestUri = string.Format(@"{0}xml/get/eq2/character/?locationdata.world={1}&c:show=id&name.first={2}", baseUrl, world, toon);
                    UseWaitCursor = true;
                    var response = await client.GetAsync(requestUri/*, ct*/);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var serializer = new XmlSerializer(typeof(Toons));
                            Toons toons = (Toons)serializer.Deserialize(stream);
                            if (toons.character != null)
                                id = toons.character.id;
                        }
                    }
                    else
                    {
                        FlexibleMessageBox.Show(this,"GetCharId Response: " + response.ReasonPhrase, "Error");
                    }
                }
                catch(Exception cidx)
                {
                    FlexibleMessageBox.Show(this,"Char id exception: " + cidx.Message, "Error");
                }
                UseWaitCursor = false;
            }
            return id;
        }

        public async Task<List<string>> GetItemId(string find)
        {
            List<string> id = new List<string>();
            string item = Uri.EscapeUriString(find);
            if (!string.IsNullOrEmpty(item))
            {
                try
                {
                    var requestUri = string.Format(@"{0}xml/get/eq2/item/?c:show=id&c:limit=10&displayname_lower={1}", baseUrl, item);
                    //UseWaitCursor = true;
                    var response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var serializer = new XmlSerializer(typeof(Items));
                            Items items = (Items)serializer.Deserialize(stream);
                            if (items.item != null)
                            {
                                foreach (Item i in items.item)
                                {
                                    id.Add(i.id);
                                }
                            }
                        }
                    }
                    else
                    {
                        FlexibleMessageBox.Show(this,"GetItemId Response: " + response.ReasonPhrase, "Error");
                    }
                    //UseWaitCursor = false;
                }
                catch(Exception gidx)
                {
                    FlexibleMessageBox.Show(this,"Get ID exception: " + gidx.Message, "Error");
                }
            }
            return id;
        }

        public async Task<ItemDetails> GetItemDetails(string id, CancellationToken ct)
        {
            ItemDetails item = null;
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var requestUri = string.Format(@"{0}xml/get/eq2/item/?id={1}", baseUrl, id);
                    //UseWaitCursor = true;
                    var response = await client.GetAsync(requestUri, ct);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var serializer = new XmlSerializer(typeof(ItemDetails));
                            item = (ItemDetails)serializer.Deserialize(stream);
                        }
                    }
                    else
                    {
                        FlexibleMessageBox.Show(this,"GetItemDetails Response: " + response.ReasonPhrase, "Error");
                    }
                    //UseWaitCursor = false;
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("GetItemDetails cancelled");
                }
                catch (Exception gidx)
                {
                    FlexibleMessageBox.Show(this,"Get Item exception: " + gidx.Message, "Error");
                }
            }
            return item;
        }

        public async Task<ItemDetail> GetItemDetails(string id)
        {
            ItemDetail item = null;
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var requestUri = string.Format(@"{0}xml/get/eq2/item/?id={1}", baseUrl, id);
                    //UseWaitCursor = true;
                    var response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var serializer = new XmlSerializer(typeof(ItemDetails));
                            ItemDetails items = (ItemDetails)serializer.Deserialize(stream);
                            if (items != null)
                                if (items.item != null)
                                    item = items.item;
                        }
                    }
                    else
                    {
                        FlexibleMessageBox.Show(this,"GetItemDetails<ItemDetail> Response: " + response.ReasonPhrase, "Error");
                    }
                    //UseWaitCursor = false;
                }
                catch (Exception gidx)
                {
                    FlexibleMessageBox.Show(this,"GetItemDetails<ItemDetail> exception: " + gidx.Message, "Error");
                }
            }
            return item;
        }

        public async Task GetImage(string id, ImageList list, CancellationToken ct)
        {
            try
            {
                bool found = false;
                if (checkBoxGameIcons.Checked)
                {
                    int iconId = int.Parse(id);
                    int iconFileNum = (iconId / iconsPerDDS) + 1;
                    int iconNum = iconId % iconsPerDDS;
                    string iconFileName = $"icon_is{iconFileNum}.dds";
                    string iconFile = Path.Combine(iconPath, iconFileName);
                    DDSImage dds = null;
                    if (!ddsImages.TryGetValue(iconId, out dds))
                    {
                        if(File.Exists(iconFile))
                        {
                            using (var stream = new FileStream(iconFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                if (stream != null)
                                {
                                    byte[] ba = new byte[stream.Length];
                                    stream.Read(ba, 0, ba.Length);
                                    dds = new DDSImage(ba);
                                    ddsImages.Add(iconId, dds);
                                }
                            }
                        }
                    }
                    if(dds != null)
                    {
                        int iconRow = iconNum / 6;
                        int iconCol = iconNum - (iconRow * 6);
                        Rectangle rect = new Rectangle(iconCol * iconSize,
                            iconRow * iconSize,
                            iconSize, iconSize);
                        list.Images.Add(dds.images[0].Clone(rect, dds.images[0].PixelFormat));
                        found = true;
                    }
                }
                if(!found)
                {
                    var requestUri = baseUrl + @"img/eq2/icons/" + id + @"/item/";
                    //UseWaitCursor = true;
                    var response = await client.GetAsync(requestUri, ct);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            Image image = Image.FromStream(stream);
                            list.Images.Add(image);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("GetImage for id " + id + ": " + response.ReasonPhrase);
                        if (id != "11")
                        {
                            //use the green monster for missing icons
                            await GetImage("11", list, ct);
                        }
                    }
                    //UseWaitCursor = false;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("GetImage cancelled");
            }
            catch (Exception gix)
            {
                FlexibleMessageBox.Show(this,"GetImage exception: " + gix.Message, "Error");
            }
        }

        public async Task<Image> GetImage(string id)
        {
            Image image = null;

            ImageList imageList = new ImageList();
            await GetImage(id, imageList, new CancellationToken());
            if(imageList.Images.Count == 0)
                return image;
            else
                return imageList.Images[0];
        }

        private async Task SetDetails(Reference refr)
        {

            if (refr.image == null)
            {
                refr.image = await GetImage(refr.icon);
            }
            if (refr.itemDetail == null)
            {
                refr.itemDetail = await GetItemDetails(refr.id);
            }
        }

        private async Task GetImageList(Collection col, ImageList images, CancellationToken ct)
        {
            foreach (Reference refr in col.referenceList.reference)
            {
                if (refr.image == null)
                {
                    await GetImage(refr.icon, images, ct);
                    if (images.Images.Count > 0)
                        refr.image = images.Images[images.Images.Count - 1];
                }
                else
                    images.Images.Add(refr.image);

                if (ct.IsCancellationRequested)
                    break;
            }
            if (col.rewardList.reward.itemList != null
                && !ct.IsCancellationRequested)
            {
                foreach (Item item in col.rewardList.reward.itemList.item)
                {
                    if (item.itemDetail == null)
                    {
                        ItemDetails detail = await GetItemDetails(item.id, ct);
                        if (ct.IsCancellationRequested)
                            break;
                        if (detail != null)
                        {
                            if (detail.item != null)
                            {
                                item.itemDetail = detail.item;
                            }
                        }
                    }
                    if (item.itemDetail != null)
                    {
                        if (item.image == null)
                        {
                            await GetImage(item.itemDetail.iconid, images, ct);
                            if(images.Images.Count > 0)
                                item.image = images.Images[images.Images.Count - 1];
                        }
                        else
                            images.Images.Add(item.image);
                    }
                    else
                    {
                        //undefined item, probably not discovered yet
                        //give it a monster icon
                        await GetImage("11", images, ct);
                        if(images.Images.Count > 0)
                            item.image = images.Images[images.Images.Count - 1];
                    }
                }
            }
            if(col.rewardList.reward.selectedItemList != null
                && !ct.IsCancellationRequested)
            {
                foreach (Item item in col.rewardList.reward.selectedItemList.selectedItem)
                {
                    if (item.itemDetail == null)
                    {
                        ItemDetails detail = await GetItemDetails(item.id, ct);
                        if (ct.IsCancellationRequested)
                            break;
                        if (detail != null)
                        {
                            if (detail.item != null)
                            {
                                item.itemDetail = detail.item;
                            }
                        }
                    }
                    if (item.itemDetail != null)
                    {
                        if (item.image == null)
                        {
                            await GetImage(item.itemDetail.iconid, images, ct);
                            if (images.Images.Count > 0)
                                item.image = images.Images[images.Images.Count - 1];
                        }
                        else
                            images.Images.Add(item.image);
                    }
                    else
                    {
                        //undefined item, probably not discovered yet
                        //give it a monster icon
                        await GetImage("11", images, ct);
                        if(images.Images.Count > 0)
                            item.image = images.Images[images.Images.Count - 1];
                    }
                }
            }
        }

        public async Task GetWorlds()
        {
            FormWait formWait = new FormWait("Game worlds census query running");
            formWait.Show(this);

            try
            {
                var requestUri = baseUrl + @"xml/get/eq2/world/?c:limit=100";
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var serializer = new XmlSerializer(typeof(Worlds));
                        worldList = (Worlds)serializer.Deserialize(stream);
                    }
                }
                else
                {
                    FlexibleMessageBox.Show(this,"GetWorlds Response " + response.ReasonPhrase, "Error");
                }
            }
            catch (Exception gwx)
            {
                FlexibleMessageBox.Show(this,"Get Worlds exception: " + gwx.Message, "Error");
            }

            formWait.Close();
        }

        private async void GetGameCollections()
        {
            UseWaitCursor = true;
            FormWait formWait = new FormWait("Collection census query running");
            if(needHeirarchy)
                formWait.Show(this);

            if (catMap == null)
            {
                flatItems = new Dictionary<string, Reference>();
                flatRewards = new Dictionary<string, Item>();
                lookupItems = new Dictionary<string, Reference>();
                categories = new Dictionary<string, List<Collection>>();
                await GetCategories();
                if(catCollectionList == null)
                {
                    FlexibleMessageBox.Show(this,"Could not fetch collection categories.\nPress [Rebuild] to try again.", "Error");
                    UseWaitCursor = false;
                    return;
                }
                catMap = catCollectionList.collection.ToDictionary(node => node.id);

                //remove duplicate category names
                consolodatedCats = (
                        from o in catCollectionList.collection
                        orderby o.ToString()
                        group o by o.ToString() into g
                        select g.First()
                        ).ToList();
            }
            else
            {
                //just sort what we already have
                if (checkBoxLevelSort.Checked)
                    sortedCats = new SortedList<string, List<Collection>>(categories, new CategoryLevelComparer());
                else
                    sortedCats = new SortedList<string, List<Collection>>(categories); //alpha sort
            }

            if (needHeirarchy)
            {
                toolStripProgressBar1.Maximum = maxCollections > 0 ? maxCollections : consolodatedCats.Count;
                toolStripProgressBar1.Value = 0;

                foreach (CatCollection c in consolodatedCats)
                {
                    await GetCollection(c);

                    //Visions of Vetrovia removed the reward items from the census.
                    //But parents' item names are the names of the child collections.
                    //So try to add collectable rewards just by matching the names
                    if (c.category.Equals("Visions of Vetrovia"))
                    {
                        foreach (Collection node in collectionList.collection)
                        {
                            //check for missing rewards
                            if (node.rewardList.reward.itemList == null)
                            {
                                //verify that the collection has items in it
                                if (node.referenceList != null)
                                {
                                    bool found = false;
                                    //If the first item in this collection is the name of another collection
                                    //then this collection is a parent.
                                    //If it's a parent, step through all the items in the parent looking for collections
                                    //with those item names.
                                    //When found, the child's collection reward is an item with the parent's item id
                                    for (int i=0; i<node.referenceList.reference.Count; i++)
                                    {
                                        string searching = node.referenceList.reference[i].name.ToLower();
                                        foreach (Collection maybeChild in collectionList.collection)
                                        {
                                            if (maybeChild.name.ToLower().Equals(searching))
                                            {
                                                // add a reward to the child collection
                                                // that is an item with the id from the parent collection
                                                if(maybeChild.rewardList.reward.itemList == null)
                                                    maybeChild.rewardList.reward.itemList = new ItemList();
                                                if(maybeChild.rewardList.reward.itemList.item == null)
                                                    maybeChild.rewardList.reward.itemList.item = new List<Item>();
                                                Item it = new Item();
                                                it.id = node.referenceList.reference[i].id; //need the ID from the parent
                                                it.quantity = "1";
                                                maybeChild.rewardList.reward.itemList.item.Add(it);
                                                found = true;
                                                break; //found the "searching" collection
                                            }
                                        }
                                        if (i == 0 && !found)
                                            break; //node is not a parent if the first item is not a collection name
                                    }
                                }
                            }
                        }
                    }

                    //find heirarchy within this collection list
                    Dictionary<string, Item> rewardTrack = new Dictionary<string, Item>();
                    Dictionary<string, Reference> itemTrack = new Dictionary<string, Reference>();
                    //build dictionaries of all items and rewards in this collection category
                    foreach (Collection node in collectionList.collection)
                    {
                        if (node.rewardList.reward.itemList != null)
                        {
                            var reward = node.rewardList.reward.itemList.item.ToDictionary(r => r.id);
                            if (reward != null)
                            {
                                //there are cases where the same reward is given for different collections
                                // so we allow duplicate keys
                                reward.ToList().ForEach(x => { x.Value.collectionId = node.id; rewardTrack[x.Key] = x.Value; });

                                //debug - which rewards are already in the list?
                                //foreach(Item i in reward.Values)
                                //{
                                //    if(rewardTrack.ContainsKey(i.id))
                                //    {
                                //        Debug.WriteLine("rewardTrack already contains " + c.category + ": "  + node.name + " " + i.id);
                                //    }
                                //}
                            }
                        }
                        if (node.referenceList != null)
                        {
                            var item = node.referenceList.reference.ToDictionary(i => i.id);
                            if (item != null)
                            {
                                try
                                {
                                    //item.ToList().ForEach(x => { x.Value.collectionId = node.id; itemTrack.Add(x.Key, x.Value); });
                                    //no known duplicate keys, but just to avoid future crashes, allow them
                                    item.ToList().ForEach(x => { x.Value.collectionId = node.id; itemTrack[x.Key] = x.Value; });

                                    //include in the list for use with finding a toon's collected items
                                    // the key is "collectionID-itemID"
                                    item.ToList().ForEach(x => lookupItems.Add(node.id + "-" + x.Key, x.Value));
                                }
                                catch { } // this has crashed when census was flakey, just survive it - should have already popped an error up
                            }
                        }
                    }
                    //search for rewards that show up as items within this list of collections
                    var nodeMap = collectionList.collection.ToDictionary(node => node.id);
                    foreach (Item award in rewardTrack.Values)
                    {
                        Reference item;
                        if (itemTrack.TryGetValue(award.id, out item))
                        {
                            //found the reward as a collection item
                            item.isReward = true;
                            //set the award to have a parent of the item's collection
                            Collection child;
                            if (nodeMap.TryGetValue(award.collectionId, out child))
                            {
                                Collection parent;
                                if(nodeMap.TryGetValue(item.collectionId, out parent))
                                {
                                    child.parent_id = parent.id;
                                }
                            }
                        }
                    }

                    //add this collection the the entire game flat lists
                    // so we can later mark rewards that are in some other category
                    //These dictionaries cannot be used to look up what a toon has collected
                    // because the same item id can go into multiple collections
                    // and when we read a toon, it has to specifiy which collection(s) they have put the item into
                    if(rewardTrack.Count > 0)
                        rewardTrack.ToList().ForEach(x => flatRewards[x.Key] = x.Value);
                    if (itemTrack.Count > 0)
                        itemTrack.ToList().ForEach(x => flatItems[x.Key] = x.Value);

                    //find root nodes in this category
                    //and add children to their parents
                    var rootNodes = new List<Collection>();
                    foreach (Collection node in collectionList.collection)
                    {
                        
                        if (!node.referenceList.sorted)
                        {
                            node.referenceList.reference.Sort();
                            node.referenceList.sorted = true;
                        }
                        Collection parent;
                        try
                        {
                            if (string.IsNullOrEmpty(node.parent_id))
                            {
                                rootNodes.Add(node);
                                if (rootNodes.Count > 1)
                                    rootNodes.Sort();
                            }
                            else if (nodeMap.TryGetValue(node.parent_id, out parent))
                            {
                                parent.children.Add(node);
                                if (parent.children.Count > 1)
                                    parent.children.Sort();
                            }
                            else
                                rootNodes.Add(node);
                        }
                        catch (Exception ex)
                        {
                            FlexibleMessageBox.Show(this,"rootnode: " + ex.Message, "Error");
                        }
                    }

                    //add this category's collections to our list
                    if (categories.ContainsKey(c.ToString()))
                        FlexibleMessageBox.Show(this,"Already have " + c.ToString(), "Error"); //should not get here
                    else
                        categories.Add(c.ToString(), rootNodes);
                    toolStripProgressBar1.Value++;

                    //debug - cut the loop short to speed up debug?
                    if (maxCollections > 0 && categories.Count >= maxCollections)
                        break;
                }
                if (checkBoxLevelSort.Checked)
                    sortedCats = new SortedList<string, List<Collection>>(categories, new CategoryLevelComparer());
                else
                    sortedCats = new SortedList<string, List<Collection>>(categories); //alpha sort

                //find and mark rewards that are a collectable in a different heirarchy node
                // (not sure this will catch all cases since the dictionaries discarded duplicate keys)
                foreach(string id in flatRewards.Keys)
                {
                    //look for this reward id as a collectable item id
                    Reference item;
                    if(flatItems.TryGetValue(id, out item))
                    {
                        if(!item.isReward)
                            item.isReward = true; //should only get here if item & reward are in different categories
                    }
                }
                toolStripStatusLabelColCount.Text = toolStripProgressBar1.Value.ToString() + " Categories";
            }

            DrawTree();

            needHeirarchy = false;

            buttonChar.Enabled = true;
            buttonRebuild.Enabled = true;

            formWait.Close();
            UseWaitCursor = false;
        }

        #endregion HTTP

        #region UI Events

        private void buttonChar_Click(object sender, EventArgs e)
        {
            CharClickHandler();
        }

        private void textBoxChar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                CharClickHandler();
            }
        }

        private async void CharClickHandler()
        {
            UseWaitCursor = true;

            await UncheckAllNodes(treeView1.Nodes);

            string id = await GetCharId();

            if (!string.IsNullOrEmpty(id))
            {
                var requestUri = string.Format(@"{0}xml/get/eq2/character_misc?id={1}&c:show=collection_list", baseUrl, id);
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var serializer = new XmlSerializer(typeof(CharacterMiscList));
                        charMiscList = (CharacterMiscList)serializer.Deserialize(stream);
                    }
                }
                else
                {
                    FlexibleMessageBox.Show(this,"GetChar Response: " + response.ReasonPhrase, "Error");
                }

                if (categories.Count > 0)
                {
                    toolStripStatusLabelItemCount.Visible = true;
                    toonColCount = 0;

                    //for each collection the toon has
                    foreach (CharMiscCollection col in charMiscList.characterMisc.charMiscColList.charMiscCollection)
                    {
                        //use the id to find that collection's name in the game collections
                        CatCollection catcol;
                        if (catMap.TryGetValue(col.crc, out catcol))
                        {
                            //use the category name to find the collection item list
                            List<Collection> collist;
                            if (categories.TryGetValue(catcol.ToString(), out collist))
                            {
                                //look at each item the toon has in that collection
                                foreach (CharMiscItem item in col.charMiscItemList.charMiscItem)
                                {
                                    //find that item in the game list
                                    //RecurseFindChildItem(item.crc, collist);
                                    Reference refr;
                                    if(lookupItems.TryGetValue(col.crc + "-" + item.crc, out refr))
                                    {
                                        refr.have = true;
                                        toonColCount++;
                                    }
                                    else
                                    {
                                        FlexibleMessageBox.Show(this,"Character has an item id " 
                                            + item.crc 
                                            + "\nin a collection with id "
                                            + col.crc
                                            + "\nthat is not in the dictionary", "Error");
                                    }
                                }
                            }
                        }
                    }

                    //update the view
                    UpdateTreeChecks();

                    toolStripStatusLabelItemCount.Text = "Collected "
                        + toonColCount.ToString()
                        + " of "
                        + lookupItems.Count.ToString()
                        + " items";
                    checkBoxHide.Enabled = true;
                }
            }
            else
            {
                FlexibleMessageBox.Show(this,"No such character found.\nCheck spelling and World", "Error");
            }
            UseWaitCursor = false;

            Cursor.Current = Cursors.Default;
        }

        private async void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView tv = sender as TreeView;
            if (e.Node.IsSelected)
            {

                tv.SelectedNode.ImageIndex = 0;
                tv.SelectedNode.SelectedImageIndex = 1;
                tv.HideSelection = true;

                // *** If a process is already underway, cancel it.
                if (cts != null)
                {
                    cts.Cancel();
                    //wait for the cancel to propagate
                    while (cts != null)
                        Debug.WriteLine("select waiting for cancel");
                }

                // *** Now set cts to cancel the current process if an item is chosen again.
                CancellationTokenSource newCTS = new CancellationTokenSource();
                cts = newCTS;

                UseWaitCursor = true;

                listView1.Items.Clear();

                Collection col = e.Node.Tag as Collection;
                if (col != null)
                {
                    try
                    {
                        if (checkBoxShowIcons.Checked)
                        {
                            ImageList images = new ImageList();
                            await GetImageList(col, images, cts.Token);
                            listView1.LargeImageList = images;
                            listView1.SmallImageList = images;
                        }
                        int i = 0;
                        enableList = true;
                        if (!cts.IsCancellationRequested)
                        {
                            foreach (Reference refr in col.referenceList.reference)
                            {

                                ListViewItem item;
                                item = new ListViewItem(refr.name);
                                item.Checked = refr.have;
                                item.Tag = refr.id;
                                if (checkBoxShowIcons.Checked)
                                    item.ImageIndex = i++;

                                item.Group = listView1.Groups[0];

                                if (refr.itemDetail == null)
                                {
                                    ItemDetails details = await GetItemDetails(refr.id, cts.Token);
                                    if (cts.IsCancellationRequested)
                                        break;
                                    if (details != null)
                                    {
                                        if (details.item != null)
                                        {
                                            refr.itemDetail = details.item;
                                        }
                                    }
                                }
                                if (refr.itemDetail != null)
                                {
                                    item.SubItems.Add(refr.itemDetail.flags.notrade.value.Equals("1") ? "X" : "");
                                    item.SubItems.Add(refr.itemDetail.flags.lore.value.Equals("1") ? "X" : "");
                                    item.SubItems.Add(refr.itemDetail.flags.heirloom.value.Equals("1") ? "X" : "");
                                    item.SubItems.Add(new ListViewItem.ListViewSubItem() { Name = "RewardItem", Text = refr.isReward ? "X" : "" });
                                }

                                listView1.Items.Add(item);

                            }
                            if (col.rewardList.reward.itemList != null
                                && !cts.IsCancellationRequested)
                            {
                                bool have = col.HaveAllItems();
                                foreach (Item item in col.rewardList.reward.itemList.item)
                                {
                                    if (item.itemDetail == null)
                                    {
                                        ItemDetails detail = await GetItemDetails(item.id, cts.Token);
                                        if (detail != null)
                                        {
                                            if (detail.item != null)
                                            {
                                                item.itemDetail = detail.item;
                                            }
                                        }
                                    }
                                    if (item.itemDetail != null)
                                    {
                                        ListViewItem lvi;
                                        lvi = new ListViewItem(item.itemDetail.displayname);
                                        lvi.Group = listView1.Groups[1];
                                        lvi.Tag = item.id;
                                        lvi.SubItems.Add(item.itemDetail.flags.notrade.value.Equals("1") ? "X" : "");
                                        lvi.SubItems.Add(item.itemDetail.flags.lore.value.Equals("1") ? "X" : "");
                                        lvi.SubItems.Add(item.itemDetail.flags.heirloom.value.Equals("1") ? "X" : "");
                                        if (checkBoxShowIcons.Checked)
                                            lvi.ImageIndex = i++;
                                        lvi.Checked = have;
                                        listView1.Items.Add(lvi);
                                    }
                                    else
                                    {
                                        ListViewItem lvi;
                                        lvi = new ListViewItem("Unknown / Undiscovered Item");
                                        lvi.Group = listView1.Groups[2];
                                        lvi.Tag = item.id;
                                        if (checkBoxShowIcons.Checked)
                                            lvi.ImageIndex = i++;
                                        listView1.Items.Add(lvi);
                                    }
                                }
                            }
                            if(col.rewardList.reward.selectedItemList != null
                                && !cts.IsCancellationRequested)
                            {
                                foreach (Item item in col.rewardList.reward.selectedItemList.selectedItem)
                                {
                                    if (item.itemDetail == null)
                                    {
                                        ItemDetails detail = await GetItemDetails(item.id, cts.Token);
                                        if (detail != null)
                                        {
                                            if (detail.item != null)
                                            {
                                                item.itemDetail = detail.item;
                                            }
                                        }
                                    }
                                    if (item.itemDetail != null)
                                    {
                                        ListViewItem lvi;
                                        lvi = new ListViewItem(item.itemDetail.displayname);
                                        lvi.Group = listView1.Groups[2];
                                        lvi.Tag = item.id;
                                        lvi.SubItems.Add(item.itemDetail.flags.notrade.value.Equals("1") ? "X" : "");
                                        lvi.SubItems.Add(item.itemDetail.flags.lore.value.Equals("1") ? "X" : "");
                                        lvi.SubItems.Add(item.itemDetail.flags.heirloom.value.Equals("1") ? "X" : "");
                                        if (checkBoxShowIcons.Checked)
                                            lvi.ImageIndex = i++;
                                        listView1.Items.Add(lvi);
                                    }
                                    else
                                    {
                                        ListViewItem lvi;
                                        lvi = new ListViewItem("Unknown / Undiscovered Item");
                                        lvi.Group = listView1.Groups[2];
                                        lvi.Tag = item.id;
                                        if (checkBoxShowIcons.Checked)
                                            lvi.ImageIndex = i++;
                                        listView1.Items.Add(lvi);
                                    }
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine("treeView after select cancelled");
                    }
                    listView1.Columns[0].Width = -2;
                    if (findNextItemIndex >= 0 && foundItems.Count > 0)
                    {
                        if (tv.SelectedNode.Equals(foundItems[findNextItemIndex].treeNode))
                        {
                            var item = listView1.Items[foundItems[findNextItemIndex].itemIndex];
                            if (item != null)
                            {
                                item.Selected = true;
                                item.EnsureVisible();
                                //Debug.WriteLine("selected found item " + foundNodes[findNextIndex].itemIndex.ToString());
                            }
                        }
                    }
                    enableList = false;
                }
                UseWaitCursor = false;
                Cursor.Current = Cursors.Default;

                // *** When the process is complete, signal that another process can proceed.
                if (cts == newCTS)
                    cts = null;
            }
        }

        private void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            //don't let the user change the checkboxes
            if (!enableTree)
                e.Cancel = true;
        }

        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //don't let the user change the checkboxes
            if (!enableList)
                e.NewValue = e.CurrentValue;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (items[0].Tag != null)
            {
                string id = items[0].Tag.ToString();
                System.Diagnostics.Process.Start("https://u.eq2wire.com/item/index/" + id);
            }
        }

        private void checkBoxLevelSort_CheckedChanged(object sender, EventArgs e)
        {
            GetGameCollections();
            UpdateTreeChecks();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Owner = this;
            about.StartPosition = FormStartPosition.CenterParent;
            about.Show();
        }

        private void findItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (findItemForm == null)
            {
                findItemForm = new FindForm();
                findItemForm.Owner = this;
                findItemForm.OnFind += new EventHandler(OnFind);
                findItemForm.OnFindNext += new EventHandler(OnFindNext);
                findItemForm.OnFindClosed += new EventHandler(OnFindClosed);
                if (listView1.SelectedItems.Count > 0)
                    findItemForm.SetText(listView1.SelectedItems[0].Text);
                findItemForm.Show();
            }
            else
            {
                //use existing form
                if (listView1.SelectedItems.Count > 0)
                    findItemForm.SetText(listView1.SelectedItems[0].Text);
                findItemForm.Focus();
            }
        }

        private void findCollectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (findCollectionForm == null)
            {
                findCollectionForm = new FindForm();
                findCollectionForm.Owner = this;
                findCollectionForm.Text = "Find Collection";
                findCollectionForm.OnFind += new EventHandler(OnFindCollection);
                findCollectionForm.OnFindNext += new EventHandler(OnFindNextCollection);
                findCollectionForm.OnFindClosed += new EventHandler(OnFindCollectionClosed);
                findCollectionForm.Show();
            }
            else
            {
                //use existing form
                findCollectionForm.Focus();
            }
        }

        private void OnFind(object sender, EventArgs eventArgs)
        {
            findNextItemIndex = 0;
            FindEventArgs args = eventArgs as FindEventArgs;
            FindClickHandler(args.name.ToLower());
        }

        private void OnFindCollection(object sender, EventArgs eventArgs)
        {
            findNextCollectionIndex = 0;
            foundItems = new List<FoundItems>(); //just clear it
            FindEventArgs args = eventArgs as FindEventArgs;
            TreeView tv = treeView1;
            if (treeView2.Visible)
                tv = treeView2;
            TreeNode[] found = tv.Nodes.Find(args.name.ToLower(), true);
            foundCollections = found.ToList();
            if (found.Length > 0)
                tv.SelectedNode = found[0];
            else
                FlexibleMessageBox.Show(this, "Not found. Check spellling."
                    + "\n\nThe tool does not find partial matches."
                    + "\nThe Name must match a collection name in the game."
                    , "Find");
        }

        private void OnFindNext(object sender, EventArgs eventArgs)
        {
            TreeView tv = treeView1;
            if (treeView2.Visible)
                tv = treeView2;
            findNextItemIndex++;
            if (foundItems.Count > findNextItemIndex)
            {
                tv.SelectedNode = null; //force a refresh in case it's the same page as last time
                tv.SelectedNode = foundItems[findNextItemIndex].treeNode;
            }
            else
            {
                FlexibleMessageBox.Show(this,"No more matching items.", "Find");
                findNextItemIndex = -1;
            }
        }

        private void OnFindNextCollection(object sender, EventArgs eventArgs)
        {
            TreeView tv = treeView1;
            if (treeView2.Visible)
                tv = treeView2;
            findNextCollectionIndex++;
            if (foundCollections.Count > findNextCollectionIndex)
            {
                tv.SelectedNode = foundCollections[findNextCollectionIndex];
            }
            else
            {
                FlexibleMessageBox.Show(this,"No more matching collections.", "Find");
                findNextCollectionIndex = -1;
            }
        }

        private void OnFindClosed(object sender, EventArgs eventArgs)
        {
            findItemForm = null;
        }

        private void OnFindCollectionClosed(object sender, EventArgs eventArgs)
        {
            findCollectionForm = null;
        }

        private async void FindClickHandler(string find)
        {
            UseWaitCursor = true;
            foundItems = new List<FoundItems>();
            try
            {
                List<string> ids = await GetItemId(find);
                if (ids.Count == 0)
                {
                    FlexibleMessageBox.Show(this,"No such item. Check spelling." 
                        + "\n\nThe tool does not find partial matches."
                        + "\nThe Name must match an item in the game."
                        , "Find");
                }
                else
                {
                    TreeView tv = treeView1;
                    if (treeView2.Visible)
                        tv = treeView2;
                    foreach (string id in ids)
                    {
                        foreach (TreeNode treeNode in tv.Nodes)
                        {
                            FindItemNode(id, treeNode);
                        }
                        if (foundItems.Count == 0)
                            FlexibleMessageBox.Show(this,"Not found", "Find");
                        else
                            tv.SelectedNode = foundItems[0].treeNode;
                    }
                }
            }
            catch (Exception iex)
            {
                FlexibleMessageBox.Show(this,"Find item exception: " + iex.Message, "Error");
            }
            UseWaitCursor = false;
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeView tv = sender as TreeView;
            if (e.Button == MouseButtons.Right)
            {
                clickedNode = e.Node;
                contextMenuStrip1.Show(tv, e.Location);
            }
        }

        private async void whatsMissingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            wantList = new List<Reference>();
            RecursiveTreeWants(clickedNode);

            foreach (Reference refr in wantList)
            {
                await SetDetails(refr);
            }

            string title = $"{clickedNode.Text} - {textBoxChar.Text}";
            WantList wants = new WantList(title, wantList, checkBoxShowIcons.Checked);
            wants.Owner = this;
            wants.Show();
            UseWaitCursor = false;
        }

        private void listView1_Enter(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    item.SubItems[0].BackColor = Color.Empty;
                    item.SubItems[0].ForeColor = Color.Empty;
                    //Debug.WriteLine("enter: set empty color for " + item.Text);
                }
            }
            //else
            //    Debug.WriteLine("no selected items");
        }

        private void listView1_Leave(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    item.SubItems[0].BackColor = SystemColors.Highlight;
                    item.SubItems[0].ForeColor = Color.White;
                    //Debug.WriteLine("leave: set highlight color for " + item.Text);
                }
                listView1.Refresh();
            }
            //else
            //    Debug.WriteLine("no selected items");
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                //Debug.WriteLine("highlight selected item " + e.Item.Text);
                e.Item.SubItems[0].BackColor = SystemColors.Highlight;
                e.Item.SubItems[0].ForeColor = Color.White;
            }
            else
            {
                //Debug.WriteLine("un-selected item " + e.Item.Text);
                e.Item.BackColor = Color.Empty;
                e.Item.ForeColor = Color.Empty;
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                var builder = new StringBuilder();
                foreach (ListViewItem item in listView1.SelectedItems)
                    builder.AppendLine(item.Text);

                Clipboard.SetText(builder.ToString());
            }
        }

        private void checkBoxHide_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked)
            {
                treeView1.Visible = false;
                treeView2.Visible = true;
            }
            else
            {
                treeView1.Visible = true;
                treeView2.Visible = false;
            }
            listView1.Items.Clear();
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                bool clearTip = true;
                if (!item.Checked && item.SubItems.Count > 4)
                {
                    if (item.Selected && item.SubItems["RewardItem"].Text.Equals("X"))
                    {
                        if (string.IsNullOrEmpty(toolTip1.GetToolTip(listView1)))
                        {
                            toolTip1.SetToolTip(listView1, "<Ctrl>-f to search for the source of this item");
                            Debug.WriteLine("tip ctrl-f");
                        }
                        clearTip = false;
                    }
                }
                if(clearTip)
                {
                    if (!string.IsNullOrEmpty(toolTip1.GetToolTip(listView1)))
                    {
                        toolTip1.SetToolTip(listView1, string.Empty);
                        Debug.WriteLine("tip none");
                    }
                }
            }
        }

        private void buttonRebuild_Click(object sender, EventArgs e)
        {
            needHeirarchy = true;
            catMap = null;
            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();
            listView1.Items.Clear();
            checkBoxHide.Checked = false;
            checkBoxHide.Enabled = false;
            buttonChar.Enabled = false;
            buttonRebuild.Enabled = false;
            toolStripStatusLabelItemCount.Visible = false;

            GetGameCollections();
        }

        private void projectWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start($"https://github.com/{githubOwner}/{githubProject}#everquest-ii-collection-utility");
            }
            catch (Exception ex)
            {
                FlexibleMessageBox.Show(this, ex.Message);
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Version localVersion = this.GetType().Assembly.GetName().Version;
                Task<Version> vtask = Task.Run(() => { return GetRemoteVersionAsync(); });
                vtask.Wait();
                remoteVersion = vtask.Result;
                string Lver = $"{localVersion.Major}.{localVersion.Minor}.{localVersion.Build}";
                string Rver = $"{remoteVersion.Major}.{remoteVersion.Minor}.{remoteVersion.Build}";
                if (remoteVersion > localVersion)
                {
                    string msg = $"Version {Rver} is available. Update?\n\n(You are running version {Lver})";
                    if(FlexibleMessageBox.Show(this, msg, 
                        "Update Available", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        UseWaitCursor = true;
                        FormWait formWait = new FormWait("Downloading and updating");
                        formWait.Show();
                        Task<FileInfo> ftask = Task.Run(() => { return GetRemoteFileAsync(); });
                        ftask.Wait();
                        if (ftask.Result != null)
                        {
                            string exeFullName = Application.ExecutablePath;
                            string oldName = exeFullName + ".old";
                            if(File.Exists(oldName))
                                File.Delete(oldName);
                            File.Move(exeFullName, oldName); //can move a running exe file
                            ZipFile.ExtractToDirectory(ftask.Result.FullName, Path.GetDirectoryName(exeFullName));
                            File.Delete(ftask.Result.FullName);
                            Application.DoEvents();
                            Application.Restart();
                        }
                    }
                }
                else
                {
                    FlexibleMessageBox.Show(this, "You are running the current version: " + Lver, "No update");
                }
            }
            catch (Exception ex)
            {
                FlexibleMessageBox.Show(this, "Could not get version information.\n" + ex.Message);
            }
        }

        private async Task<Version> GetRemoteVersionAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    ProductInfoHeaderValue hdr = new ProductInfoHeaderValue(githubProject, "1");
                    client.DefaultRequestHeaders.UserAgent.Add(hdr);
                    HttpResponseMessage response = await client.GetAsync($"https://api.github.com/repos/{githubOwner}/{githubProject}/releases/latest");
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Regex reVer = new Regex(@"""tag_name.:.v(\d+)\.(\d+)\.(\d+)(\.(\d+))?""");
                        Match match = reVer.Match(responseBody);
                        if (match.Success)
                        {
                            int major = Int32.Parse(match.Groups[1].Value);
                            int minor = Int32.Parse(match.Groups[2].Value);
                            int build = Int32.Parse(match.Groups[3].Value);
                            if (string.IsNullOrEmpty(match.Groups[5].Value))
                                return new Version(major, minor, build, 0);
                            else
                            {
                                int revision = Int32.Parse(match.Groups[5].Value);
                                return new Version(major, minor, build, revision);
                            }
                        }
                    }
                    return new Version("0.0.0.0");
                }
            }
            catch { return new Version("0.0.0.0"); }
        }

        private async Task<FileInfo> GetRemoteFileAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    ProductInfoHeaderValue hdr = new ProductInfoHeaderValue(githubProject, "1");
                    client.DefaultRequestHeaders.UserAgent.Add(hdr);
                    string dl = $"https://github.com/{githubOwner}/{githubProject}/releases/latest/download/{githubProject}.zip";
                    HttpResponseMessage response = await client.GetAsync(dl);
                    if (response.IsSuccessStatusCode)
                    {
                        byte[] responseBody = await response.Content.ReadAsByteArrayAsync();
                        string tmp = Path.GetTempFileName();
                        File.WriteAllBytes(tmp, responseBody);
                        FileInfo fi = new FileInfo(tmp);
                        return fi;
                    }
                }
                return null;
            }
            catch { return null; }
        }

        private void checkBoxGameIcons_CheckedChanged(object sender, EventArgs e)
        {
            if (initializing)
                return;

            if (checkBoxGameIcons.Checked)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.ShowNewFolderButton = false;
                    fbd.Description = @"Select your 'Everquest II' game folder, where the 'everquest2.exe' file resides.";
                    fbd.SelectedPath = Settings.Default.GameFolder;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        iconPath = Path.Combine(fbd.SelectedPath, iconFolder);
                        if (File.Exists(Path.Combine(iconPath, "icon_is1.dds")))
                        {
                            Settings.Default.GameFolder = fbd.SelectedPath;
                            Settings.Default.UseGameIcons = true;
                        }
                        else
                        {
                            FlexibleMessageBox.Show(this, "Could not find 'Everquest II\\UI\\Default\\images\\icons' at that location");
                            checkBoxGameIcons.Checked = false;
                        }
                    }
                }
            }
            else
            {
                Settings.Default.UseGameIcons = false;
            }
        }

        #endregion UI Events

        public async void FindItemNode(string id, TreeNode treeNode)
        {
            if (treeNode.Tag != null)
            {
                Collection col;
                col = treeNode.Tag as Collection;
                if (col != null)
                {
                    int itemIndex = 0;
                    foreach(Reference refr in col.referenceList.reference)
                    {
                        if(refr.id.Equals(id))
                        {
                            FoundItems found = new FoundItems();
                            found.treeNode = treeNode;
                            found.itemName = refr.name;
                            found.itemIndex = itemIndex;
                            foundItems.Add(found);
                            break;
                        }
                        itemIndex++;
                    }
                    itemIndex = col.referenceList.reference.Count;
                    if (col.rewardList != null)
                    {
                        if (col.rewardList.reward.itemList != null)
                        {
                            foreach (Item item in col.rewardList.reward.itemList.item)
                            {
                                if (item.id.Equals(id))
                                {
                                    if(item.itemDetail == null)
                                    {
                                        item.itemDetail = await GetItemDetails(item.id);
                                    }
                                    if (item.itemDetail != null)
                                    {
                                        FoundItems found = new FoundItems();
                                        found.treeNode = treeNode;
                                        found.itemName = item.itemDetail.displayname;
                                        found.itemIndex = itemIndex;
                                        foundItems.Add(found);
                                    }
                                    else
                                    {
                                        Debug.WriteLine("id match but no details");
                                    }
                                }
                                itemIndex++;
                            }
                        }
                    }
                }
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                FindItemNode(id, tn);
            }

        }

        private void DrawTree()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            foreach (string cat in sortedCats.Keys)
            {
                List<Collection> listCol;
                TreeNode node = treeView1.Nodes.Add(cat.ToLower(), cat);
                if (categories.TryGetValue(cat, out listCol))
                {
                    node.Tag = listCol;
                    PopulateTree(listCol, node);
                }
            }
            treeView1.EndUpdate();
        }

        private void PopulateTree(List<Collection> nodes, TreeNode parent)
        {
            foreach (Collection col in nodes)
            {
                TreeNode child = parent.Nodes.Add(col.ToString().ToLower(), col.ToString());
                child.Tag = col;
                if(col.children.Count > 0)
                    PopulateTree(col.children, child);
            }
        }

        private async void UpdateTreeChecks()
        {
            finishedNodes.Clear();
            enableTree = true; //allow checkboxes to be changed
            foreach (TreeNode tn in treeView1.Nodes)
            {
                RecursiveTreeChecks(tn);
            }

            //clone for the hide completions tree
            treeView2.Nodes.Clear();
            foreach (TreeNode node in treeView1.Nodes)
            {
                treeView2.Nodes.Add((TreeNode)node.Clone());
            }
            //now remove finished nodes
            foreach (TreeNode checkedNode in finishedNodes)
{
                TreeNode[] found = treeView2.Nodes.Find(checkedNode.Name, true);
                //if (found.Length > 1)
                //    Debug.WriteLine("debug");
                foreach(TreeNode treeNode in found)
                {
                    //just double check that they are the same node 
                    // since the census contains some oddities
                    if (checkedNode.Parent != null && treeNode.Parent != null)
                    {
                        //we can verify the collection ID on child nodes
                        Collection delCol = checkedNode.Tag as Collection;
                        Collection treeCol = treeNode.Tag as Collection;
                        if (delCol != null && treeCol != null)
                        {
                            if (delCol.id == treeCol.id)
                                treeNode.Remove();
                        }
                    }
                    else
                    {
                        //on top level nodes, a matching category name is a matching node
                        if(treeNode.Level == 0 && checkedNode.Level == 0)
                            treeNode.Remove();
                    }
                }
            }

            if (treeView1.Visible)
                await UpdateSelectedNodeChecks();
            else
                listView1.Items.Clear(); //we rebuilt the tree, nothing is selected

            enableTree = false;
        }

        private void RecursiveTreeChecks(TreeNode treeNode)
        {
            if (treeNode.Tag != null)
            {
                Collection col;
                col = treeNode.Tag as Collection;
                if (col != null)
                {
                    bool all = col.HaveAllItems();
                    treeNode.Checked = all;
                    if (all)
                        finishedNodes.Add(treeNode); //save for the hide completed tree
                    if(!all && col.HaveAnyItems())
                    {
                        treeNode.ForeColor = Color.Blue;
                        treeNode.BackColor = Color.White;
                    }
                    else
                    {
                        treeNode.ForeColor = Color.Empty;
                        treeNode.BackColor = Color.Empty;
                    }
                }
                else
                {
                    //top level node is a list
                    List<Collection> listCol = treeNode.Tag as List<Collection>;
                    if(listCol != null)
                    {
                        bool ck = true;
                        bool color = false;
                        foreach (Collection c in listCol)
                        {
                            ck &= c.HaveAllItems();
                            color |= c.HaveAnyItems();
                        }
                        treeNode.Checked = ck;
                        if (ck)
                            finishedNodes.Add(treeNode);
                        if (!ck && color)
                        {
                            treeNode.ForeColor = Color.Blue;
                            treeNode.BackColor = Color.White;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Empty;
                            treeNode.BackColor = Color.Empty;
                        }
                    }
                }
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                RecursiveTreeChecks(tn);
            }
        }

        private void RecursiveTreeWants(TreeNode treeNode)
        {
            if (treeNode.Tag != null)
            {
                Collection col;
                col = treeNode.Tag as Collection;
                if (col != null)
                {
                    foreach (Reference refr in col.referenceList.reference)
                    {
                        if (!refr.have)
                        {
                            wantList.Add(refr);
                        }
                    }
                }
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                RecursiveTreeWants(tn);
            }
        }

        private void RecursiveTreeUnChecks(TreeNode treeNode)
        {
            if (treeNode.Tag != null)
            {
                Collection col;
                col = treeNode.Tag as Collection;
                if (col != null)
                {
                    treeNode.Checked = false;
                    col.SetHaveNoItems();
                }
                else
                {
                    //top level node is a list
                    List<Collection> listCol = treeNode.Tag as List<Collection>;
                    if (listCol != null)
                    {
                        foreach (Collection c in listCol)
                        {
                            c.SetHaveNoItems();
                        }
                        treeNode.Checked = false;
                    }
                }
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                RecursiveTreeUnChecks(tn);
            }
        }

        public async Task UncheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = false;
                RecursiveTreeUnChecks(node);
            }
            treeView1.Refresh();
            await UpdateSelectedNodeChecks();
        }

        private async Task UpdateSelectedNodeChecks()
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                Collection col = node.Tag as Collection;
                if(col == null)
                {
                    //this is a list of collections, nothing to do
                    return;
                }

                enableList = true;
                if (col != null)
                {
                    foreach (Reference refr in col.referenceList.reference)
                    {
                        ListViewItem item = listView1.FindItemWithText(refr.name);
                        if (item != null)
                        {
                            item.Checked = refr.have;
                        }
                    }
                    listView1.Refresh();
                }
                if (col.rewardList.reward.itemList != null)
                {
                    // *** If a process is already underway, cancel it.
                    if (cts != null)
                    {
                        cts.Cancel();
                    }
                    // *** Now set cts to cancel the current process if the process runs again.
                    CancellationTokenSource newCTS = new CancellationTokenSource();
                    cts = newCTS;


                    bool haveAll = col.HaveAllItems();
                    foreach (Item item in col.rewardList.reward.itemList.item)
                    {
                        if (item.itemDetail == null)
                        {
                            ItemDetails detail = await GetItemDetails(item.id, cts.Token);
                            if (detail != null)
                            {
                                if (detail.item != null)
                                {
                                    item.itemDetail = detail.item;
                                }
                            }
                        }
                        if(item.itemDetail != null)
                        {
                            ListViewItem reward = listView1.FindItemWithText(item.itemDetail.displayname);
                            if (reward != null)
                            {
                                reward.Checked = haveAll;
                            }
                        }
                    }

                    // *** When the process is complete, signal that another process can proceed.
                    if (cts == newCTS)
                        cts = null;

                }
                enableList = false;

            }

        }

        private static DateTime UnixTimeStampToDateTime(string unixTimeString)
        {
            DateTime dtDateTime;
            //this is problematic in non-english
            // so try some stuff
            // and just give up if nothing works such that all old servers will be listed
            bool worked = false;
            try
            {
                double seconds;
                Double.TryParse(unixTimeString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out seconds);
                dtDateTime = new DateTime(1970, 1, 1);
                dtDateTime = dtDateTime.AddSeconds(seconds).ToLocalTime();
                return dtDateTime;
            }
            catch { } //just fail

            if (!worked)
            {
                try
                {
                    string[] ints = unixTimeString.Split(new char[] { ',', '.' });
                    Int64 unixTimeStamp = Int64.Parse(ints[0]);
                    // Unix timestamp is seconds past epoch
                    dtDateTime = new DateTime(1970, 1, 1);
                    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                    return dtDateTime;
                }
                catch
                {
                    //gets here with German time settings,
                    // just end up listing all the servers, including dead ones
                    return DateTime.Now;

                }
            }
            return DateTime.Now; //will list all servers
        }

        public static int CompareStringsWithNumbers(string a, string b)
        {
            //only useful when the strings are similar, like
            // 1st Lieutenant Dergud, 279 AS - Page 10
            // 1st Lieutenant Dergud, 279 AS - Page 4

            string[] anums = Regex.Split(a, @"\D+");
            string[] bnums = Regex.Split(b, @"\D+");

            int aLen = 0;
            int acount = 0;
            int bLen = 0;
            int bcount = 0;
            foreach(string s in anums)
            {
                if (s.Length > aLen)
                {
                    aLen = s.Length;
                    acount++;
                }
            }
            foreach (string s in bnums)
            {
                if (s.Length > bLen)
                {
                    bLen = s.Length;
                    bcount++;
                }
            }

            if (aLen == 0 || bLen == 0 || acount != bcount)
                return a.CompareTo(b);

            int maxLen = aLen > bLen ? aLen : bLen;

            //pad spaces to all numbers so they sort properly
            string[] apads = new string[anums.Length];
            string[] bpads = new string[bnums.Length];
            for (int i = 0; i < anums.Length; i++)
            {
                if(anums[i].Length > 0)
                    apads[i] = anums[i].PadLeft(maxLen, ' ');
            }
            for (int i = 0; i < bnums.Length; i++)
            {
                if(bnums[i].Length > 0)
                    bpads[i] = bnums[i].PadLeft(maxLen, ' ');
            }

            //replace numbers in the strings
            // (this could be a problem if the string has the same number in it more than once)
            string afix = a;
            for (int i = 0; i < anums.Length; i++)
            {
                if(anums[i].Length > 0)
                    afix = a.Replace(anums[i], apads[i]);
            }
            string bfix = b;
            for (int i = 0; i < bnums.Length; i++)
            {
                if(bnums[i].Length > 0)
                    bfix = b.Replace(bnums[i], bpads[i]);
            }
            return afix.CompareTo(bfix);
        }

        class CategoryLevelComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int xNumStart = x.IndexOf('(') + 1;
                int xCloseParen = x.IndexOf(')');
                int yNumStart = y.IndexOf('(') + 1;
                int yCloseParen = y.IndexOf(')');
                string xNum = x.Substring(xNumStart, xCloseParen - xNumStart);
                string yNum = y.Substring(yNumStart, yCloseParen - yNumStart);
                if(!xNum.Equals(yNum))
                    return xNum.CompareTo(yNum);
                return x.CompareTo(y); //numbers are the same, use the whole string
            }
        }
    }
}
