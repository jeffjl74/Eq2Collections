namespace Eq2Collections
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonRebuild = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxChar = new System.Windows.Forms.TextBox();
            this.comboBoxWorlds = new System.Windows.Forms.ComboBox();
            this.buttonChar = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxShowIcons = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.checkBoxHide = new System.Windows.Forms.CheckBox();
            this.checkBoxLevelSort = new System.Windows.Forms.CheckBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RewardItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelColCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelItemCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.whatsMissingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxGameIcons = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonRebuild);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBoxChar);
            this.panel1.Controls.Add(this.comboBoxWorlds);
            this.panel1.Controls.Add(this.buttonChar);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(776, 85);
            this.panel1.TabIndex = 0;
            // 
            // buttonRebuild
            // 
            this.buttonRebuild.Enabled = false;
            this.buttonRebuild.Location = new System.Drawing.Point(115, 60);
            this.buttonRebuild.Name = "buttonRebuild";
            this.buttonRebuild.Size = new System.Drawing.Size(73, 23);
            this.buttonRebuild.TabIndex = 10;
            this.buttonRebuild.Text = "Rebuild";
            this.toolTip1.SetToolTip(this.buttonRebuild, "Clear the categories and rebuild from the census");
            this.buttonRebuild.UseVisualStyleBackColor = true;
            this.buttonRebuild.Click += new System.EventHandler(this.buttonRebuild_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Character:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Collection Categories";
            // 
            // textBoxChar
            // 
            this.textBoxChar.Location = new System.Drawing.Point(69, 31);
            this.textBoxChar.Name = "textBoxChar";
            this.textBoxChar.Size = new System.Drawing.Size(120, 20);
            this.textBoxChar.TabIndex = 4;
            this.textBoxChar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxChar_KeyPress);
            // 
            // comboBoxWorlds
            // 
            this.comboBoxWorlds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWorlds.FormattingEnabled = true;
            this.comboBoxWorlds.Location = new System.Drawing.Point(194, 30);
            this.comboBoxWorlds.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxWorlds.Name = "comboBoxWorlds";
            this.comboBoxWorlds.Size = new System.Drawing.Size(86, 21);
            this.comboBoxWorlds.TabIndex = 3;
            // 
            // buttonChar
            // 
            this.buttonChar.Enabled = false;
            this.buttonChar.Location = new System.Drawing.Point(285, 29);
            this.buttonChar.Name = "buttonChar";
            this.buttonChar.Size = new System.Drawing.Size(75, 23);
            this.buttonChar.TabIndex = 1;
            this.buttonChar.Text = "Get";
            this.buttonChar.UseVisualStyleBackColor = true;
            this.buttonChar.Click += new System.EventHandler(this.buttonChar_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(776, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findItemToolStripMenuItem,
            this.findCollectionToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // findItemToolStripMenuItem
            // 
            this.findItemToolStripMenuItem.Name = "findItemToolStripMenuItem";
            this.findItemToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findItemToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.findItemToolStripMenuItem.Text = "Find Item...";
            this.findItemToolStripMenuItem.Click += new System.EventHandler(this.findItemToolStripMenuItem_Click);
            // 
            // findCollectionToolStripMenuItem
            // 
            this.findCollectionToolStripMenuItem.Name = "findCollectionToolStripMenuItem";
            this.findCollectionToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.findCollectionToolStripMenuItem.Text = "Find Collection...";
            this.findCollectionToolStripMenuItem.Click += new System.EventHandler(this.findCollectionToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdateToolStripMenuItem,
            this.projectWebsiteToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // checkForUpdateToolStripMenuItem
            // 
            this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            this.checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.checkForUpdateToolStripMenuItem.Text = "Check for update...";
            this.checkForUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdateToolStripMenuItem_Click);
            // 
            // projectWebsiteToolStripMenuItem
            // 
            this.projectWebsiteToolStripMenuItem.Name = "projectWebsiteToolStripMenuItem";
            this.projectWebsiteToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.projectWebsiteToolStripMenuItem.Text = "Project Website";
            this.projectWebsiteToolStripMenuItem.Click += new System.EventHandler(this.projectWebsiteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // checkBoxShowIcons
            // 
            this.checkBoxShowIcons.AutoSize = true;
            this.checkBoxShowIcons.Checked = true;
            this.checkBoxShowIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowIcons.Location = new System.Drawing.Point(233, 5);
            this.checkBoxShowIcons.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxShowIcons.Name = "checkBoxShowIcons";
            this.checkBoxShowIcons.Size = new System.Drawing.Size(82, 17);
            this.checkBoxShowIcons.TabIndex = 2;
            this.checkBoxShowIcons.Text = "Show Icons";
            this.checkBoxShowIcons.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 85);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(776, 218);
            this.panel2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView2);
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1.Controls.Add(this.panel4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Size = new System.Drawing.Size(776, 218);
            this.splitContainer1.SplitterDistance = 209;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView2
            // 
            this.treeView2.CheckBoxes = true;
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView2.Location = new System.Drawing.Point(0, 28);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(209, 190);
            this.treeView2.TabIndex = 11;
            this.treeView2.Visible = false;
            this.treeView2.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCheck);
            this.treeView2.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView2.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 28);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(209, 190);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCheck);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.checkBoxHide);
            this.panel4.Controls.Add(this.checkBoxLevelSort);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(209, 28);
            this.panel4.TabIndex = 10;
            // 
            // checkBoxHide
            // 
            this.checkBoxHide.AutoSize = true;
            this.checkBoxHide.Enabled = false;
            this.checkBoxHide.Location = new System.Drawing.Point(105, 6);
            this.checkBoxHide.Name = "checkBoxHide";
            this.checkBoxHide.Size = new System.Drawing.Size(101, 17);
            this.checkBoxHide.TabIndex = 10;
            this.checkBoxHide.Text = "Hide Completed";
            this.checkBoxHide.UseVisualStyleBackColor = true;
            this.checkBoxHide.CheckedChanged += new System.EventHandler(this.checkBoxHide_CheckedChanged);
            // 
            // checkBoxLevelSort
            // 
            this.checkBoxLevelSort.AutoSize = true;
            this.checkBoxLevelSort.Checked = true;
            this.checkBoxLevelSort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLevelSort.Location = new System.Drawing.Point(10, 6);
            this.checkBoxLevelSort.Name = "checkBoxLevelSort";
            this.checkBoxLevelSort.Size = new System.Drawing.Size(88, 17);
            this.checkBoxLevelSort.TabIndex = 9;
            this.checkBoxLevelSort.Text = "Sort by Level";
            this.checkBoxLevelSort.UseVisualStyleBackColor = true;
            this.checkBoxLevelSort.CheckedChanged += new System.EventHandler(this.checkBoxLevelSort_CheckedChanged);
            // 
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.RewardItem});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 28);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(563, 190);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView1_ItemCheck);
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listView1.Enter += new System.EventHandler(this.listView1_Enter);
            this.listView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyUp);
            this.listView1.Leave += new System.EventHandler(this.listView1_Leave);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            this.listView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseMove);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Item";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "No Trade";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Lore";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Heirloom";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RewardItem
            // 
            this.RewardItem.Text = "Reward Item";
            this.RewardItem.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RewardItem.Width = 80;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.checkBoxGameIcons);
            this.panel3.Controls.Add(this.checkBoxShowIcons);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(563, 28);
            this.panel3.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(225, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Collection Items (double click for eq2wire.com)";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabelColCount,
            this.toolStripStatusLabelItemCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 303);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 7, 0);
            this.statusStrip1.Size = new System.Drawing.Size(776, 24);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 18);
            // 
            // toolStripStatusLabelColCount
            // 
            this.toolStripStatusLabelColCount.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelColCount.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.toolStripStatusLabelColCount.Name = "toolStripStatusLabelColCount";
            this.toolStripStatusLabelColCount.Size = new System.Drawing.Size(67, 19);
            this.toolStripStatusLabelColCount.Text = "Categories";
            // 
            // toolStripStatusLabelItemCount
            // 
            this.toolStripStatusLabelItemCount.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelItemCount.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.toolStripStatusLabelItemCount.Name = "toolStripStatusLabelItemCount";
            this.toolStripStatusLabelItemCount.Size = new System.Drawing.Size(40, 19);
            this.toolStripStatusLabelItemCount.Text = "Items";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whatsMissingToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(164, 26);
            // 
            // whatsMissingToolStripMenuItem
            // 
            this.whatsMissingToolStripMenuItem.Name = "whatsMissingToolStripMenuItem";
            this.whatsMissingToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.whatsMissingToolStripMenuItem.Text = "What\'s Missing...";
            this.whatsMissingToolStripMenuItem.Click += new System.EventHandler(this.whatsMissingToolStripMenuItem_Click);
            // 
            // checkBoxGameIcons
            // 
            this.checkBoxGameIcons.AutoSize = true;
            this.checkBoxGameIcons.Location = new System.Drawing.Point(321, 5);
            this.checkBoxGameIcons.Name = "checkBoxGameIcons";
            this.checkBoxGameIcons.Size = new System.Drawing.Size(102, 17);
            this.checkBoxGameIcons.TabIndex = 3;
            this.checkBoxGameIcons.Text = "Use game icons";
            this.toolTip1.SetToolTip(this.checkBoxGameIcons, "Check to browse to the game folder\r\nand use the game icons instead of census icon" +
        "s");
            this.checkBoxGameIcons.UseVisualStyleBackColor = true;
            this.checkBoxGameIcons.CheckedChanged += new System.EventHandler(this.checkBoxGameIcons_CheckedChanged);
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonChar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 327);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(532, 366);
            this.Name = "Form1";
            this.Text = "EQII Collections";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button buttonChar;
        private System.Windows.Forms.CheckBox checkBoxShowIcons;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ComboBox comboBoxWorlds;
        private System.Windows.Forms.TextBox textBoxChar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxLevelSort;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem whatsMissingToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader RewardItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelColCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelItemCount;
        private System.Windows.Forms.ToolStripMenuItem findCollectionToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxHide;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonRebuild;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectWebsiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.CheckBox checkBoxGameIcons;
    }
}

