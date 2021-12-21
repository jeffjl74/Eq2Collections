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
    public partial class FormWait : Form
    {
        List<string> feedback;
        int feedbackIndex = 0;
        System.Timers.Timer timer = new System.Timers.Timer();
        string prompt;

        public FormWait(string msg)
        {
            InitializeComponent();
            prompt = msg;
        }

        private void FormWait_Shown(object sender, EventArgs e)
        {
            this.CenterToParent();

            UseWaitCursor = true;
            feedback = new List<string>();
            feedback.Add(prompt + ".");
            feedback.Add(prompt + "..");
            feedback.Add(prompt + "...");
            label1.Text = feedback[0];
            feedbackIndex = 1;
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.SynchronizingObject = this;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            label1.Text = feedback[feedbackIndex];
            feedbackIndex++;
            if (feedbackIndex >= feedback.Count)
                feedbackIndex = 0;
        }
    }
}
