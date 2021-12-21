using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eq2Collections
{
    public partial class FindForm : Form
    {
        public EventHandler OnFind;
        public EventHandler OnFindNext;
        public EventHandler OnFindClosed;

        enum eState { FIND, FIND_NEXT }
        eState state = eState.FIND;

        public FindForm()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            textBoxName.Text = text;
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (state == eState.FIND)
            {
                OnFind?.Invoke(this, new FindEventArgs(textBoxName.Text));
                state = eState.FIND_NEXT;
                buttonFind.Text = "Find Next";
            }
            else
            {
                OnFindNext?.Invoke(this, new FindEventArgs(textBoxName.Text));
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            state = eState.FIND;
            buttonFind.Text = "Find";
        }

        private void FindForm_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.FindSize.IsEmpty)
                this.Size = Properties.Settings.Default.FindSize;

            //position relative to the parent
            Point clientPt = Properties.Settings.Default.FindLocation;
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

        private void FindForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save position relative to the parent
            Point clientPt = this.Owner.PointToClient(this.Location);

            Properties.Settings.Default.FindLocation = clientPt;
            Properties.Settings.Default.FindSize = this.Size;
            Properties.Settings.Default.Save();
            OnFindClosed?.Invoke(this, new FindEventArgs(""));
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
