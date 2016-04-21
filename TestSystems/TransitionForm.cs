using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BddSharp.Kernel;
using BddSharp.Serializer;

namespace TransitionSystems
{
    public partial class TransitionForm : Form
    {

        public TransitionForm() 
        {
            InitializeComponent();
        }

        public void RunTransitionSystem()
        {
           //valg af test
            MilnersScheduler ms = new MilnersScheduler(int.Parse(textN.Text));
            OutputBox.Text = ms.Run();
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            RunTransitionSystem();
            this.Cursor = Cursors.Arrow;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}