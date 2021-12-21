using System;
using System.Reflection;
using System.Windows.Forms;

namespace Eq2Collections
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            //this.textBoxDescription.Text = AssemblyDescription;
            this.textBoxDescription.Text = "Pulls collection data from the EQII census and constructs a hiearchy. "
                + "Pulls a character's collected items and checks the boxes in the hiearchy. "
                + Environment.NewLine + Environment.NewLine
                + "Top level nodes are collection category names, grouped by level. "
                + "hiearchy is set when a reward from one collection is an item in another collection."
                + "When building the hiearchy the tool does not cross levels or categories. "
                + "For example: the level 15 butterfly rewards are items in the level 20 butterfly collection. "
                + "The tool will not set this parent-child relationship since they are in different levels. "
                + "The Find Item menu may be useful in these cases. "
                + Environment.NewLine + Environment.NewLine
                + "The tool does not list coin collection rewards. "
                + Environment.NewLine + Environment.NewLine
                + "The tool just assembles the data in the census, and there are some oddities. "
                + "For example, the two 'Relics of the Ethernauts' collections under 'Shadow Odyssey ( 85)', "
                + "one of which is populated with items that don't seem to exist. "
                + Environment.NewLine + Environment.NewLine
                + "As of this writing, VoV census data does not contain reward data. "
                + "Therefore, for the Visions of Vetrovia category, "
                + "the tool builds its own parent / child relationships based on item names. "
                ;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutBox1_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }
    }
}
