using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Eq2Collections
{
    public partial class WantList : Form
    {
        private string title;
        List<Reference> references;
        private bool showIcons = false;
        bool hideRewards = false;
        bool hideTradeable = false;
        bool hideNoTrade = false;
        private int sortCol = -1;

        public WantList(string title, List<Reference> references, bool wantIcons)
        {
            InitializeComponent();

            this.title = title;
            this.references = references;
            this.showIcons = wantIcons;
        }

        private void WantList_Shown(object sender, EventArgs e)
        {
            ShowItems();

            hideRewardItemsToolStripMenuItem.Checked = hideRewards;
            hideTradeableToolStripMenuItem.Checked = hideTradeable;
            hideNoTradeToolStripMenuItem.Checked = hideNoTrade;
            this.Text = "Want List: " + title;
        }

        private void ShowItems()
        {
            if (listView1.Items.Count > 0)
                listView1.Items.Clear();

            ImageList images = new ImageList();
            foreach (Reference refr in references)
            {
                if (refr.image != null)
                    images.Images.Add(refr.image);
                else
                    Console.WriteLine("no image for " + refr.name);
            }
            listView1.LargeImageList = images;
            listView1.SmallImageList = images;

            int i = 0;
            int itemCount = 0;
            foreach (Reference refr in references)
            {
                ListViewItem item;
                item = new ListViewItem(refr.name);
                item.Checked = refr.have;
                item.Tag = refr.id;
                if (showIcons)
                    item.ImageIndex = i++;

                bool notrade = false;
                bool heirloom = false;
                if (refr.itemDetail != null)
                {
                    notrade = refr.itemDetail.flags.notrade.value.Equals("1");
                    heirloom = refr.itemDetail.flags.heirloom.value.Equals("1");

                    item.SubItems.Add(notrade ? "X" : "");
                    item.SubItems.Add(refr.itemDetail.flags.lore.value.Equals("1") ? "X" : "");
                    item.SubItems.Add(heirloom ? "X" : "");
                    item.SubItems.Add(refr.isReward ? "X" : "");
                }

                if (refr.isReward && hideRewards)
                    continue;

                if(hideTradeable && !notrade)
                    continue;

                if(hideNoTrade && notrade)
                    continue;

                listView1.Items.Add(item);
                itemCount++;
            }
            listView1.Columns[0].Width = -2;
            toolStripStatusLabelCounts.Text = string.Format("{0} item{1}", itemCount, itemCount == 0 || itemCount > 1 ? "s" : "");
        }

        private void WantList_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save position relative to the parent
            Point clientPt = this.Owner.PointToClient(this.Location);
            Properties.Settings.Default.WantsLocation = clientPt;
            Properties.Settings.Default.WantsSize = this.Size;
            Properties.Settings.Default.Save();
        }

        private void WantList_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.WantsSize.IsEmpty)
                this.Size = Properties.Settings.Default.WantsSize;

            //position relative to the parent
            Point clientPt = Properties.Settings.Default.WantsLocation;
            Point screenPt = this.Owner.PointToScreen(clientPt);
            //make sure it fits on screen in case the parent has moved
            if (screenPt.X < 0)
                screenPt.X = 0;
            if (screenPt.X + this.Size.Width > SystemInformation.VirtualScreen.Right)
                screenPt.X = SystemInformation.VirtualScreen.Right - this.Size.Width;
            if (screenPt.Y < 0)
                screenPt.Y = 0;
            if (screenPt.Y + this.Size.Height > SystemInformation.WorkingArea.Bottom)
                screenPt.Y = SystemInformation.WorkingArea.Bottom - this.Size.Height;
            this.Location = screenPt;

        }

        private void copyListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ListViewItem item in listView1.Items)
            {
                sb.Append(item.Text + ", https://u.eq2wire.com/item/index/" + item.Tag.ToString() + "\n");
            }
            Clipboard.SetText(sb.ToString());
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

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == sortCol)
            {
                // Reverse the current sort direction for this column.
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
                ((ListViewItemComparer)listView1.ListViewItemSorter).order = listView1.Sorting;
            }
            else
            {
                sortCol = e.Column;
                // Set the column number that is to be sorted; default to ascending.
                listView1.Sorting = SortOrder.Ascending;
                listView1.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView1.Sorting);
            }

            // Perform the sort with these new sort options.
            listView1.Sort();
        }

        private void hideRewardItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideRewards = !hideRewards;
            hideRewardItemsToolStripMenuItem.Checked = hideRewards;
            ShowItems();
        }

        private void hideTradeableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideTradeable = !hideTradeable;
            hideTradeableToolStripMenuItem.Checked = hideTradeable;
            ShowItems();
        }

        private void hideNoTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideNoTrade = !hideNoTrade;
            hideNoTradeToolStripMenuItem.Checked = hideNoTrade;
            ShowItems();
        }
    }
}
